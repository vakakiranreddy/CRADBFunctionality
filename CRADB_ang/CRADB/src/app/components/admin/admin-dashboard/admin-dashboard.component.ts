import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';

import { UserService } from '../../../services/user.service';
import { BookingService } from '../../../services/booking.service';
import { RoomService } from '../../../services/room.service';
import { DepartmentService } from '../../../services/department.service';
import { LocationService } from '../../../services/location.service';
import { DeskService } from '../../../services/desk.service';
import { EventService } from '../../../services/event.service';
import { AuthService } from '../../../services/auth.service';
import { AuthResponse } from '../../../models/auth.models';
import { ResourceType, BookingStatus } from '../../../models/enums';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './admin-dashboard.component.html',
  styleUrls: ['./admin-dashboard.component.css']
})
export class AdminDashboardComponent implements OnInit {
stats = {
    totalUsers: 0,
    totalRooms: 0,
    totalDesks: 0,
    activeBookings: 0,
    totalDepartments: 0,
    totalLocations: 0,
    busyRooms: 0,
    occupiedDesks: 0
  };

recentBookings: any[] = [];
  allBookings: any[] = [];
  upcomingEvents: any[] = [];
  locations: any[] = [];
  selectedLocation: any = null;
  currentDate = new Date();
  loading = false;
  calendarWeeks: any[][] = [];
  currentMonthYear = '';
  todayDate = '';
  todayEvents: any[] = [];
  currentUser: AuthResponse | null = null;
  showProfileDropdown = false;
  showBookingDropdown = false;

  constructor(
    private userService: UserService,
    private bookingService: BookingService,
    private roomService: RoomService,
    private departmentService: DepartmentService,
    private locationService: LocationService,
    private deskService: DeskService,
    private eventService: EventService,
    private authService: AuthService,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.currentUser = this.authService.getCurrentUser();
    this.generateCalendar();
    this.currentMonthYear = this.getCurrentMonth();
    this.todayDate = this.getTodayDate();
    this.loadDashboardData();
    this.loadLocations();
    this.loadUpcomingEvents();
    this.loadTodayEvents();
  }

  getUserInitials(): string {
    if (!this.currentUser) return 'AD';
    const firstInitial = this.currentUser.FirstName?.charAt(0) || '';
    const lastInitial = this.currentUser.LastName?.charAt(0) || '';
    return (firstInitial + lastInitial).toUpperCase() || 'AD';
  }

  getUserDisplayName(): string {
    if (!this.currentUser) return 'Admin';
    return `${this.currentUser.FirstName || ''} ${this.currentUser.LastName || ''}`.trim() || 'Admin';
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

  loadDashboardData(): void {
    this.loading = true;
    let completedRequests = 0;
    const totalRequests = 7;
    
    const checkAllLoaded = () => {
      completedRequests++;
      if (completedRequests >= totalRequests) {
        this.loading = false;
        this.cdr.detectChanges();
      }
    };
    
    // Load users count
    this.userService.getAllUsers().subscribe({
      next: (users: any[]) => {
        this.stats.totalUsers = users?.length || 0;
        checkAllLoaded();
      },
      error: () => {
        this.stats.totalUsers = 0;
        checkAllLoaded();
      }
    });

    // Load rooms count and busy rooms
    this.roomService.getAllRooms().subscribe({
      next: (rooms: any[]) => {
        this.stats.totalRooms = rooms?.length || 0;
        checkAllLoaded();
      },
      error: () => {
        this.stats.totalRooms = 0;
        checkAllLoaded();
      }
    });

    // Load departments count
    this.departmentService.getAllDepartments().subscribe({
      next: (departments: any[]) => {
        this.stats.totalDepartments = departments?.length || 0;
        checkAllLoaded();
      },
      error: () => {
        this.stats.totalDepartments = 0;
        checkAllLoaded();
      }
    });

    // Load desks count
    this.deskService.getAllDesks().subscribe({
      next: (desks: any[]) => {
        this.stats.totalDesks = desks?.length || 0;
        checkAllLoaded();
      },
      error: () => {
        this.stats.totalDesks = 0;
        checkAllLoaded();
      }
    });

    // Load locations count
    this.locationService.getAllLocations().subscribe({
      next: (locations: any[]) => {
        this.stats.totalLocations = locations?.length || 0;
        checkAllLoaded();
      },
      error: () => {
        this.stats.totalLocations = 0;
        checkAllLoaded();
      }
    });

    // Load bookings data
    this.bookingService.getAllBookings().subscribe({
      next: (bookings: any[]) => {
        this.allBookings = bookings || [];
        if (bookings && bookings.length > 0) {
          const now = new Date();
          const activeBookings = bookings.filter(b => {
            const startTime = new Date(b.StartTime);
            const endTime = new Date(b.EndTime);
            return now >= startTime && now <= endTime && (b.Status === BookingStatus.Confirmed || b.Status === BookingStatus.Active);
          });
          
          this.stats.activeBookings = activeBookings.length;
          
          // Calculate busy rooms and occupied desks
          const activeRoomBookings = activeBookings.filter(b => b.ResourceType === ResourceType.Room || b.ResourceType === 'Room');
          const activeDeskBookings = activeBookings.filter(b => b.ResourceType === ResourceType.Desk || b.ResourceType === 'Desk');
          
          this.stats.busyRooms = activeRoomBookings.length;
          this.stats.occupiedDesks = activeDeskBookings.length;
          
          this.recentBookings = bookings
            .filter(b => b.StartTime && b.StartTime !== '0001-01-01T00:00:00')
            .sort((a, b) => new Date(b.StartTime).getTime() - new Date(a.StartTime).getTime())
            .slice(0, 5)
            .map(booking => ({
              id: booking.BookingId || booking.Id,
              roomName: booking.ResourceName || `Resource ${booking.ResourceId}`,
              userName: booking.UserName || 'Unknown User',
              date: new Date(booking.StartTime).toLocaleDateString(),
              time: `${new Date(booking.StartTime).toLocaleTimeString()} - ${new Date(booking.EndTime).toLocaleTimeString()}`,
              status: this.getStatusText(booking.Status)
            }));
        } else {
          this.stats.activeBookings = 0;
          this.stats.busyRooms = 0;
          this.stats.occupiedDesks = 0;
          this.recentBookings = [];
        }
        checkAllLoaded();
      },
      error: () => {
        this.stats.activeBookings = 0;
        this.stats.busyRooms = 0;
        this.stats.occupiedDesks = 0;
        this.recentBookings = [];
        this.allBookings = [];
        checkAllLoaded();
      }
    });

    checkAllLoaded();
  }

  getStatusText(status: number): string {
    switch(status) {
      case BookingStatus.Pending: return 'Pending';
      case BookingStatus.Confirmed: return 'Confirmed';
      case BookingStatus.Active: return 'Active';
      case BookingStatus.Completed: return 'Completed';
      case BookingStatus.Cancelled: return 'Cancelled';
      default: return 'Unknown';
    }
  }

  loadLocations(): void {
    this.locationService.getAllLocations().subscribe({
      next: (locations: any[]) => {
        this.locations = locations || [];
        this.stats.totalLocations = locations?.length || 0;
        if (locations && locations.length > 0) {
          this.selectedLocation = locations[0];
        }
        this.cdr.detectChanges();
      },
      error: (error: any) => {
        console.error('Error loading locations:', error);
      }
    });
  }

  loadUpcomingEvents(): void {
    this.eventService.getUpcomingEvents().subscribe({
      next: (events: any[]) => {
        this.upcomingEvents = events?.slice(0, 3) || [];
        this.cdr.detectChanges();
      },
      error: () => {
        this.upcomingEvents = [];
      }
    });
  }

  loadTodayEvents(): void {
    const today = new Date().toISOString().split('T')[0];
    this.eventService.getAllEvents().subscribe({
      next: (events: any[]) => {
        const todayEvents = events.filter(event => {
          const eventDate = new Date(event.Date).toISOString().split('T')[0];
          return eventDate === today && event.IsActive;
        });
        
        this.todayEvents = todayEvents.map(event => ({
          EventId: event.EventId,
          EventTitle: event.EventTitle,
          StartTime: new Date(event.StartTime).toLocaleTimeString([], {hour: '2-digit', minute:'2-digit'}),
          EndTime: new Date(event.EndTime).toLocaleTimeString([], {hour: '2-digit', minute:'2-digit'}),
          bgColor: '#F0F8FF',
          borderColor: '#4A90E2',
          iconBg: '#4A90E2',
          icon: 'calendar-alt',
          titleColor: '#2C3E50',
          timeColor: '#7F8C8D'
        }));
        
        this.updateCalendarWithEvents(events);
        this.cdr.detectChanges();
      },
      error: () => {
        this.todayEvents = [];
      }
    });
  }

  updateCalendarWithEvents(events: any[]): void {
    if (!this.calendarWeeks.length) return;
    
    const currentMonth = this.currentDate.getMonth();
    const currentYear = this.currentDate.getFullYear();
    
    this.calendarWeeks.forEach(week => {
      week.forEach(day => {
        const dayDate = new Date(currentYear, currentMonth, day.date);
        const dayString = dayDate.toISOString().split('T')[0];
        
        day.hasEvent = events.some(event => {
          const eventDate = new Date(event.Date).toISOString().split('T')[0];
          return eventDate === dayString && event.IsActive;
        });
      });
    });
  }

  onLocationChange(event: any): void {
    const locationId = event.target.value;
    this.selectedLocation = this.locations.find(l => l.LocationId == locationId);
  }

  getCurrentMonth(): string {
    return this.currentDate.toLocaleDateString('en-US', { month: 'long', year: 'numeric' });
  }

getTodayDate(): string {
    return this.currentDate.toLocaleDateString('en-US', { 
      weekday: 'long', 
      day: 'numeric', 
      month: 'long', 
      year: 'numeric' 
    });
  }



  generateCalendar(): void {
    const year = this.currentDate.getFullYear();
    const month = this.currentDate.getMonth();
    const firstDay = new Date(year, month, 1);
    const startDate = new Date(firstDay);
    startDate.setDate(startDate.getDate() - firstDay.getDay());
    
    this.calendarWeeks = [];
    const today = new Date();
    
    for (let week = 0; week < 6; week++) {
      const weekDays = [];
      for (let day = 0; day < 7; day++) {
        const currentDay = new Date(startDate);
        currentDay.setDate(startDate.getDate() + (week * 7) + day);
        
        weekDays.push({
          date: currentDay.getDate(),
          isToday: currentDay.toDateString() === today.toDateString(),
          isCurrentMonth: currentDay.getMonth() === month,
          hasEvent: false
        });
      }
      this.calendarWeeks.push(weekDays);
    }
  }

  previousMonth(): void {
    this.currentDate.setMonth(this.currentDate.getMonth() - 1);
    this.generateCalendar();
    this.currentMonthYear = this.getCurrentMonth();
    this.loadTodayEvents();
  }

  nextMonth(): void {
    this.currentDate.setMonth(this.currentDate.getMonth() + 1);
    this.generateCalendar();
    this.currentMonthYear = this.getCurrentMonth();
    this.loadTodayEvents();
  }

  toggleBookingDropdown(): void {
    this.showBookingDropdown = !this.showBookingDropdown;
  }

  filterBookings(type: string): void {
    this.showBookingDropdown = false;
    switch(type) {
      case 'room':
        this.router.navigate(['/admin/room-bookings']);
        break;
      case 'desk':
        this.router.navigate(['/admin/desk-bookings']);
        break;
      case 'my':
        this.router.navigate(['/admin/my-bookings']);
        break;
    }
  }
}