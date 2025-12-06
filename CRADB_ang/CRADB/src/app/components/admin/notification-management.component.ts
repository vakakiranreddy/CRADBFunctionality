import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule, Router } from '@angular/router';
import { NotificationService, SendBroadcastDto } from '../../services/notification.service';
import { LocationService, LocationResponse } from '../../services/location.service';
import { UserService } from '../../services/user.service';
import { AuthService } from '../../services/auth.service';
import { AuthResponse } from '../../models/auth.models';

@Component({
  selector: 'app-notification-management',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './notification-management.component.html',
  styleUrls: ['./notification-management.component.css']
})
export class NotificationManagementComponent implements OnInit {
  broadcastForm: SendBroadcastDto = {
    Title: '',
    Message: '',
    TargetAudience: 'All',
    LocationId: undefined
  };
  
  locations: LocationResponse[] = [];
  broadcasts: any[] = [];
  sending = false;
  currentUser: AuthResponse | null = null;
  showProfileDropdown = false;
  
  constructor(
    private notificationService: NotificationService,
    private locationService: LocationService,
    private userService: UserService,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.currentUser = this.authService.getCurrentUser();
    this.loadLocations();
    this.loadBroadcasts();
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

  loadLocations(): void {
    this.locationService.getAllLocations().subscribe({
      next: (locations) => {
        this.locations = locations;
      },
      error: (error) => {
        console.error('Error loading locations:', error);
      }
    });
  }

  loadBroadcasts(): void {
    this.notificationService.getAllBroadcasts().subscribe({
      next: (broadcasts) => {
        this.broadcasts = broadcasts.slice(0, 10); // Show last 10
      },
      error: (error) => {
        console.error('Error loading broadcasts:', error);
      }
    });
  }



  sendBroadcast(): void {
    if (!this.broadcastForm.Title || !this.broadcastForm.Message) {
      alert('Please fill in all required fields');
      return;
    }

    this.sending = true;
    
    this.notificationService.createBroadcast(this.broadcastForm).subscribe({
      next: (response) => {
        this.sending = false;
        alert('Broadcast sent successfully!');
        
        // Reset form
        this.broadcastForm = {
          Title: '',
          Message: '',
          TargetAudience: 'All',
          LocationId: undefined
        };
        
        // Reload data
        this.loadBroadcasts();
      },
      error: (error) => {
        this.sending = false;
        console.error('Error sending broadcast:', error);
        alert('Failed to send broadcast. Please try again.');
      }
    });
  }

  formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString() + ' ' + date.toLocaleTimeString([], {hour: '2-digit', minute:'2-digit'});
  }
}