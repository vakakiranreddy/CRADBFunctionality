import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, FormsModule, Validators } from '@angular/forms';
import { SidebarComponent } from '../../layout/sidebar/sidebar.component';
import { DeskService } from '../../../services/desk.service';
import { DeskResponse } from '../../../models/desk.models';
import { ResourceService, MaintenanceStatusDto, BlockResourceDto } from '../../../services/resource.service';
import { AuthService } from '../../../services/auth.service';
import { AuthResponse } from '../../../models/auth.models';

@Component({
  selector: 'app-desks',
  standalone: true,
  imports: [CommonModule, SidebarComponent, RouterModule, ReactiveFormsModule, FormsModule],
  templateUrl: './desks.component.html',
  styleUrls: ['./desks.component.css']
})
export class DesksComponent implements OnInit {
  desks: DeskResponse[] = [];
  filteredDesks: DeskResponse[] = [];
  loading = false;
  currentUser: AuthResponse | null = null;
  showProfileDropdown = false;
  editingDesk: DeskResponse | null = null;
  deskForm: FormGroup;
  saving = false;
  searchTerm = '';
  statusFilter = '';

  constructor(
    private deskService: DeskService,
    private resourceService: ResourceService,
    private authService: AuthService,
    private router: Router,
    private fb: FormBuilder,
    private cdr: ChangeDetectorRef
  ) {
    this.deskForm = this.fb.group({
      deskName: ['', Validators.required],
      hasMonitor: [false],
      hasKeyboard: [false],
      hasMouse: [false],
      hasDockingStation: [false]
    });
  }

  ngOnInit(): void {
    this.currentUser = this.authService.getCurrentUser();
    this.loadDesks();
  }

  loadDesks(): void {
    this.loading = true;
    this.deskService.getAllDesks().subscribe({
      next: (desks: DeskResponse[]) => {
        this.desks = desks || [];
        this.filteredDesks = [...this.desks];
        this.applyFilters();
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: (error: any) => {
        console.error('Error loading desks:', error);
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  applyFilters(): void {
    let filtered = [...this.desks];

    if (this.searchTerm) {
      filtered = filtered.filter(desk =>
        desk.DeskName.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
        desk.LocationName?.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
        desk.BuildingName?.toLowerCase().includes(this.searchTerm.toLowerCase())
      );
    }

    if (this.statusFilter === 'available') {
      filtered = filtered.filter(d => !d.IsBlocked && !d.IsUnderMaintenance);
    } else if (this.statusFilter === 'maintenance') {
      filtered = filtered.filter(d => d.IsUnderMaintenance);
    } else if (this.statusFilter === 'blocked') {
      filtered = filtered.filter(d => d.IsBlocked);
    }

    this.filteredDesks = filtered;
  }

  getUserInitials(): string {
    if (!this.currentUser) return 'AD';
    const firstInitial = this.currentUser.FirstName?.charAt(0) || '';
    const lastInitial = this.currentUser.LastName?.charAt(0) || '';
    return (firstInitial + lastInitial).toUpperCase() || 'AD';
  }

  getUserRole(): string {
    return this.currentUser?.Role || 'Admin';
  }

  toggleProfileDropdown(): void {
    this.showProfileDropdown = !this.showProfileDropdown;
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  getImageUrl(imageData: any): string {
    if (!imageData) return '';
    if (typeof imageData === 'string') {
      if (imageData.startsWith('data:image')) {
        return imageData;
      }
      return `data:image/jpeg;base64,${imageData}`;
    }
    return '';
  }

  toggleMaintenance(desk: DeskResponse): void {
    const dto: MaintenanceStatusDto = {
      IsUnderMaintenance: !desk.IsUnderMaintenance
    };

    this.resourceService.updateMaintenanceStatus(desk.ResourceId, dto).subscribe({
      next: () => {
        this.loadDesks();
      },
      error: (error) => {
        console.error('Error toggling maintenance:', error);
        alert('Failed to update maintenance status');
      }
    });
  }

  toggleBlock(desk: DeskResponse): void {
    if (desk.IsBlocked) {
      this.resourceService.unblockResource(desk.ResourceId).subscribe({
        next: () => {
          this.loadDesks();
        },
        error: (error) => {
          console.error('Error unblocking resource:', error);
          alert('Failed to unblock resource');
        }
      });
    } else {
      const reason = prompt('Enter block reason (optional):');
      const dto: BlockResourceDto = {
        BlockReason: reason || undefined
      };

      this.resourceService.blockResource(desk.ResourceId, dto).subscribe({
        next: () => {
          this.loadDesks();
        },
        error: (error) => {
          console.error('Error blocking resource:', error);
          alert('Failed to block resource');
        }
      });
    }
  }

  editDesk(desk: DeskResponse): void {
    this.editingDesk = desk;
    this.deskForm.patchValue({
      deskName: desk.DeskName,
      hasMonitor: desk.HasMonitor,
      hasKeyboard: desk.HasKeyboard,
      hasMouse: desk.HasMouse,
      hasDockingStation: desk.HasDockingStation
    });
  }

  cancelEdit(): void {
    this.editingDesk = null;
    this.deskForm.reset();
  }

  saveDesk(): void {
    if (this.deskForm.invalid || !this.editingDesk) return;

    this.saving = true;
    const updateDto = {
      DeskName: this.deskForm.value.deskName,
      HasMonitor: this.deskForm.value.hasMonitor,
      HasKeyboard: this.deskForm.value.hasKeyboard,
      HasMouse: this.deskForm.value.hasMouse,
      HasDockingStation: this.deskForm.value.hasDockingStation
    };

    this.deskService.updateDesk(this.editingDesk.Id, updateDto).subscribe({
      next: () => {
        this.saving = false;
        this.cancelEdit();
        this.loadDesks();
      },
      error: (error) => {
        console.error('Error updating desk:', error);
        this.saving = false;
      }
    });
  }

  deleteDesk(desk: DeskResponse): void {
    if (!confirm(`Delete desk "${desk.DeskName}"? This will also delete the resource.`)) return;

    this.resourceService.deleteResource(desk.ResourceId).subscribe({
      next: () => {
        this.loadDesks();
      },
      error: (error) => {
        console.error('Error deleting desk:', error);
        alert('Failed to delete desk');
      }
    });
  }
}