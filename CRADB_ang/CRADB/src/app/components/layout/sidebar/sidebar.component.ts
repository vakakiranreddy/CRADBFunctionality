import { Component, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.css']
})
export class SidebarComponent {
  showBookingDropdown = false;

  constructor(private authService: AuthService) {}

  get isAdmin(): boolean {
    return this.authService.isAdmin();
  }

  toggleBookingDropdown(): void {
    this.showBookingDropdown = !this.showBookingDropdown;
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: Event): void {
    const target = event.target as HTMLElement;
    const sidebar = target.closest('.sidebar');
    const bookingLink = target.closest('.nav-item');
    
    if (!sidebar || !bookingLink?.querySelector('.booking-submenu')) {
      this.showBookingDropdown = false;
    }
  }
}