import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NotificationService } from '../../services/notification.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-user-notifications',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './user-notifications.component.html',
  styleUrls: ['./user-notifications.component.css']
})
export class UserNotificationsComponent implements OnInit {
  notifications: any[] = [];
  loading = true;
  currentUser: any;

  constructor(
    private notificationService: NotificationService,
    private authService: AuthService
  ) {
    this.currentUser = this.authService.getCurrentUser();
  }

  ngOnInit(): void {
    this.loadNotifications();
  }

  get unreadCount(): number {
    return this.notifications.filter(n => !n.IsRead).length;
  }

  loadNotifications(): void {
    if (!this.currentUser?.UserId) {
      this.loading = false;
      return;
    }

    this.notificationService.getUserNotifications(this.currentUser.UserId).subscribe({
      next: (notifications) => {
        this.notifications = notifications || [];
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading notifications:', error);
        this.notifications = [];
        this.loading = false;
      }
    });
  }

  markAsRead(notificationId: number): void {
    this.notificationService.markAsRead(notificationId).subscribe({
      next: () => {
        const notification = this.notifications.find(n => n.Id === notificationId);
        if (notification) {
          notification.IsRead = true;
        }
      },
      error: (error) => {
        console.error('Error marking notification as read:', error);
      }
    });
  }

  markAllAsRead(): void {
    const unreadNotifications = this.notifications.filter(n => !n.IsRead);
    unreadNotifications.forEach(notification => {
      this.markAsRead(notification.Id);
    });
  }

  getNotificationIcon(type: string): string {
    if (!type || typeof type !== 'string') {
      return 'fa-bell';
    }
    
    switch (type.toLowerCase()) {
      case 'booking':
        return 'fa-calendar-check';
      case 'event':
        return 'fa-calendar-alt';
      case 'reminder':
        return 'fa-clock';
      case 'system':
        return 'fa-cog';
      default:
        return 'fa-bell';
    }
  }

  formatDate(dateString: string): string {
    const date = new Date(dateString);
    const now = new Date();
    const diffMs = now.getTime() - date.getTime();
    const diffMins = Math.floor(diffMs / 60000);
    const diffHours = Math.floor(diffMins / 60);
    const diffDays = Math.floor(diffHours / 24);

    if (diffMins < 1) return 'Just now';
    if (diffMins < 60) return `${diffMins}m ago`;
    if (diffHours < 24) return `${diffHours}h ago`;
    if (diffDays < 7) return `${diffDays}d ago`;
    return date.toLocaleDateString();
  }
}