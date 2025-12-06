import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../../services/auth.service';
import { NotificationService, NotificationItem, EmailNotificationRequest } from '../../../services/notification.service';
import { LocationService, LocationResponse } from '../../../services/location.service';
import { BookingService, BookingResponse } from '../../../services/booking.service';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent implements OnInit {
  currentUser: any;
  notifications: NotificationItem[] = [];
  locations: LocationResponse[] = [];
  showNotifications = false;
  showEmailModal = false;
  
  emailForm = {
    type: 'booking' as 'booking' | 'event' | 'location',
    locationId: '',
    subject: '',
    message: ''
  };

  constructor(
    private authService: AuthService,
    private notificationService: NotificationService,
    private locationService: LocationService,
    private bookingService: BookingService
  ) {
    this.currentUser = this.authService.getCurrentUser();
  }

  ngOnInit(): void {
    if (this.currentUser?.role === 'Admin') {
      this.loadNotifications();
      this.loadLocations();
    }
  }

  loadNotifications(): void {
    // Try to get user notifications first, fallback to booking data
    if (this.currentUser?.UserId) {
      this.notificationService.getUserNotifications(this.currentUser.UserId).subscribe({
        next: (notifications: any[]) => {
          this.notifications = notifications.map(n => ({
            id: n.Id,
            type: 'booking' as const,
            title: n.Title || 'Notification',
            message: n.Message || '',
            locationName: n.LocationName,
            count: 1,
            createdAt: n.CreatedAt
          }));
        },
        error: () => {
          this.loadBookingNotifications();
        }
      });
    } else {
      this.loadBookingNotifications();
    }
  }

  loadBookingNotifications(): void {
    this.bookingService.getBookingsByStatus('Pending').subscribe({
      next: (bookings: BookingResponse[]) => {
        this.notifications = bookings.map((booking, index) => ({
          id: index + 1,
          type: 'booking' as const,
          title: `Pending Booking: ${booking.ResourceName || 'Room'}`,
          message: `${booking.UserName || 'Unknown User'} - ${new Date(booking.StartTime).toLocaleDateString()}`,
          locationName: booking.LocationName,
          count: 1,
          createdAt: booking.CreatedAt
        }));
      },
      error: (error: any) => {
        console.error('Error loading booking notifications:', error);
        this.notifications = [];
      }
    });
  }

  loadLocations(): void {
    this.locationService.getAllLocations().subscribe({
      next: (locations: LocationResponse[]) => {
        this.locations = locations || [];
      }
    });
  }

  sendEmailNotification(): void {
    const broadcastDto = {
      Title: this.emailForm.subject,
      Message: this.emailForm.message,
      LocationId: this.emailForm.locationId ? parseInt(this.emailForm.locationId) : undefined,
      TargetAudience: this.emailForm.type
    };

    this.notificationService.createBroadcast(broadcastDto).subscribe({
      next: () => {
        this.showEmailModal = false;
        this.emailForm = { type: 'booking', locationId: '', subject: '', message: '' };
        alert('Broadcast notification sent successfully!');
      },
      error: (error: any) => {
        console.error('Error sending broadcast:', error);
        alert('Failed to send broadcast notification');
      }
    });
  }

  logout() {
    this.authService.logout();
  }
}