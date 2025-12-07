import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
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
  loading = false;
  currentUser: any;

  constructor(
    private notificationService: NotificationService,
    private authService: AuthService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.currentUser = this.authService.getCurrentUser();
    if(this.currentUser) {
        this.loadNotifications();
    }
  }

  loadNotifications() {
    if (!this.currentUser) return;
    this.loading = true;
    this.notificationService.getUserNotifications(this.currentUser.UserId).subscribe({
      next: (data) => {
        console.log('Notifications received:', data);
        this.notifications = data;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error(err);
        this.loading = false;
      }
    });
  }
}