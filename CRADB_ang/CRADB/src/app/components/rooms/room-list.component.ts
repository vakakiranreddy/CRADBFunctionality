import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { RoomService } from '../../services/room.service';
import { RoomResponse } from '../../models/room.models';
import { LocationService, LocationResponse } from '../../services/location.service';
import { BookingService, CreateBookingRequest } from '../../services/booking.service';
import { NavigationComponent } from '../shared/navigation.component';
import Toastify from 'toastify-js';

@Component({
  selector: 'app-room-list',
  standalone: true,
  imports: [CommonModule, FormsModule, NavigationComponent],
  templateUrl: './room-list.component.html',
  styleUrls: ['./room-list.component.css']
})
export class RoomListComponent implements OnInit {
  rooms: RoomResponse[] = [];
  locations: LocationResponse[] = [];
  loading = false;
  selectedLocation = '';
  selectedDate = '';
  employeeCount = '8';
  showBookingModal = false;
  selectedRoom: RoomResponse | null = null;
  bookingLoading = false;
  bookingData = {
    meetingName: '',
    startTime: '',
    endTime: '',
    participantCount: 1,
    purpose: ''
  };

  constructor(
    private roomService: RoomService,
    private locationService: LocationService,
    private bookingService: BookingService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadLocations();
    this.loadAllRooms();
  }

  refreshData(): void {
    this.loadLocations();
    this.loadAllRooms();
  }

  loadAllRooms(): void {
    this.loading = true;
    this.roomService.getAllRooms().subscribe({
      next: (rooms) => {
        this.rooms = this.filterRoomsByCapacity(rooms || []);
        this.loading = false;
      },
      error: (error) => {
        Toastify({
          text: "Failed to load rooms",
          duration: 3000,
          backgroundColor: "#ef4444"
        }).showToast();
        this.rooms = [];
        this.loading = false;
      }
    });
  }

  loadLocations(): void {
    this.locationService.getAllLocations().subscribe({
      next: (locations) => {
        this.locations = locations || [];
      },
      error: (error) => {
        Toastify({
          text: "Failed to load locations",
          duration: 3000,
          backgroundColor: "#ef4444"
        }).showToast();
        this.locations = [];
      }
    });
  }

  onLocationChange(): void {
    this.searchRooms();
  }

  searchRooms(): void {
    if (this.selectedLocation) {
      this.loading = true;
      this.roomService.getRoomsByLocation(Number(this.selectedLocation)).subscribe({
        next: (rooms) => {
          this.rooms = this.filterRoomsByCapacity(rooms || []);
          this.loading = false;
        },
        error: (error) => {
          Toastify({
            text: "Failed to load rooms by location",
            duration: 3000,
            backgroundColor: "#ef4444"
          }).showToast();
          this.rooms = [];
          this.loading = false;
        }
      });
    } else {
      this.loadAllRooms();
    }
  }

  filterRoomsByCapacity(rooms: RoomResponse[]): RoomResponse[] {
    if (!this.employeeCount) return rooms;
    const minCapacity = Number(this.employeeCount);
    return rooms.filter(room => room.Capacity >= minCapacity);
  }

  getLocationName(locationId: number): string {
    const location = this.locations.find(l => l.LocationId === locationId);
    return location ? location.City : 'Unknown';
  }

  openBookingModal(room: RoomResponse): void {
    this.selectedRoom = room;
    this.showBookingModal = true;
    // Set default times (current time + 1 hour)
    const now = new Date();
    const startTime = new Date(now.getTime() + 60 * 60 * 1000); // 1 hour from now
    const endTime = new Date(startTime.getTime() + 60 * 60 * 1000); // 1 hour duration
    
    this.bookingData = {
      meetingName: '',
      startTime: this.formatDateTimeLocal(startTime),
      endTime: this.formatDateTimeLocal(endTime),
      participantCount: room.Capacity,
      purpose: ''
    };
  }

  closeBookingModal(): void {
    this.showBookingModal = false;
    this.selectedRoom = null;
  }

  bookRoom(): void {
    if (!this.selectedRoom) return;
    
    this.bookingLoading = true;
    const booking: CreateBookingRequest = {
      resourceId: this.selectedRoom.ResourceId,
      startTime: new Date(this.bookingData.startTime),
      endTime: new Date(this.bookingData.endTime),
      meetingName: this.bookingData.meetingName,
      participantCount: this.bookingData.participantCount,
      purpose: this.bookingData.purpose || undefined
    };

    this.bookingService.createBooking(booking).subscribe({
      next: (response) => {
        Toastify({
          text: "Room booked successfully!",
          duration: 3000,
          backgroundColor: "#10b981"
        }).showToast();
        this.closeBookingModal();
        this.bookingLoading = false;
      },
      error: (error) => {
        Toastify({
          text: "Failed to book room. Please try again.",
          duration: 3000,
          backgroundColor: "#ef4444"
        }).showToast();
        this.bookingLoading = false;
      }
    });
  }

  private formatDateTimeLocal(date: Date): string {
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    const hours = String(date.getHours()).padStart(2, '0');
    const minutes = String(date.getMinutes()).padStart(2, '0');
    return `${year}-${month}-${day}T${hours}:${minutes}`;
  }

  getRoomImage(room: RoomResponse): string {
    const images = [
      'https://images.unsplash.com/photo-1497366216548-37526070297c?w=400',
      'https://images.unsplash.com/photo-1497366811353-6870744d04b2?w=400',
      'https://images.unsplash.com/photo-1560472354-b33ff0c44a43?w=400',
      'https://images.unsplash.com/photo-1582653291997-079a1c04e5a1?w=400'
    ];
    return images[room.ResourceId % images.length];
  }
}