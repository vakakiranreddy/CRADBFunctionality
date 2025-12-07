import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NavigationComponent } from '../shared/navigation.component';
import { AuthService } from '../../services/auth.service';
import { UserService } from '../../services/user.service';
import Toastify from 'toastify-js';

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
  imagePreview: string | null = null;
  uploadingImage = false;
  
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

  constructor(
    private authService: AuthService,
    private userService: UserService
  ) {}

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

  onFileSelected(event: any): void {
    const file = event.target.files[0];
    if (file) {
      const reader = new FileReader();
      reader.onload = (e: any) => this.imagePreview = e.target.result;
      reader.readAsDataURL(file);
      this.uploadProfileImage(file);
    }
  }

  uploadProfileImage(file: File): void {
    if (!this.currentUser) return;
    
    this.uploadingImage = true;
    this.userService.uploadProfileImage(this.currentUser.UserId, file).subscribe({
      next: (response) => {
        this.uploadingImage = false;
        
        // Update local state
        const updatedUser = { ...this.currentUser, ProfileImage: response.imageUrl };
        this.authService.updateCurrentUser(updatedUser);
        this.currentUser = updatedUser;
        
        Toastify({
          text: "Profile picture updated!",
          duration: 3000,
          backgroundColor: "#10b981"
        }).showToast();
      },
      error: (err) => {
        this.uploadingImage = false;
        console.error('Upload failed', err);
        Toastify({
          text: "Failed to upload image. Please try again.",
          duration: 3000,
          backgroundColor: "#ef4444"
        }).showToast();
      }
    });
  }

  viewBookings(): void { window.location.href = '/bookings'; }
  bookRoom(): void { window.location.href = '/rooms'; }
  viewEvents(): void { window.location.href = '/events'; }
}