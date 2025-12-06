import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NavigationComponent } from '../shared/navigation.component';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-user-profile',
  standalone: true,
  imports: [CommonModule, FormsModule, NavigationComponent],
  templateUrl: './user-profile.component.html',
  styleUrls: ['./user-profile.component.css']
})
export class UserProfileComponent implements OnInit {
  currentUser: any;
  isEditing = false;
  userProfile = {
    email: 'user@kanini.com',
    phone: '+91 9876543210',
    department: 'Engineering',
    location: 'Bangalore',
    employeeId: 'KAN001',
    joinDate: 'Jan 15, 2023',
    manager: 'John Doe',
    team: 'Frontend Development',
    notifications: 'All Notifications',
    defaultDuration: '60'
  };

  constructor(private authService: AuthService) {}

  ngOnInit(): void {
    this.currentUser = this.authService.getCurrentUser();
    if (this.currentUser) {
      this.userProfile.email = this.currentUser.Email || this.userProfile.email;
    }
  }

  getUserInitials(): string {
    if (!this.currentUser) return 'U';
    const firstName = this.currentUser.FirstName || '';
    const lastName = this.currentUser.LastName || '';
    return (firstName.charAt(0) + lastName.charAt(0)).toUpperCase();
  }

  toggleEdit(): void {
    if (this.isEditing) {
      console.log('Saving profile:', this.userProfile);
    }
    this.isEditing = !this.isEditing;
  }

  viewBookings(): void { window.location.href = '/bookings'; }
  bookRoom(): void { window.location.href = '/rooms'; }
  viewEvents(): void { window.location.href = '/events'; }
}