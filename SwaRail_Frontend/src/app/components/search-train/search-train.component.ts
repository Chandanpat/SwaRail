import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { TrainService } from '../../services/train.service';
import { ScheduleService } from '../../services/schedule.service';
import { StationService } from '../../services/station.service';
import { ReservationService } from '../../services/reservation.service';

@Component({
  selector: 'app-search-train',
  standalone: true,
  imports: [CommonModule, RouterModule, ReactiveFormsModule],
  templateUrl: './search-train.component.html',
  styleUrl: './search-train.component.css'
})
export class SearchTrainComponent implements OnInit {

  searchForm: FormGroup;
  stations: any[] = [];
  trains: any[] = [];
  searched: boolean = false;

  constructor(
    private fb: FormBuilder,
    private trainService: TrainService,
    private scheduleService: ScheduleService,
    private stationService: StationService,
    private reservationService: ReservationService,
    private router: Router
  ) {
    this.searchForm = this.fb.group({
      sourceStation: ['', Validators.required],
      destinationStation: ['', Validators.required],
      journeyDate: ['', Validators.required],
    });
  }

  ngOnInit() {
    this.loadStations();
  }

  loadStations() {
    this.stationService.getStations().subscribe({
      next: (response) => {
        this.stations = response;
      },
      error: (error) => {
        console.error('Error fetching stations:', error);
        if (error.status === 401) {
          alert('Session expired. Please log in again.');
        }
      }
    });
  }

  searchTrains() {
    if (this.searchForm.invalid) return;

    const { sourceStation, destinationStation, journeyDate } = this.searchForm.value;

    const selectedDate = new Date(journeyDate);
    const today = new Date();
    today.setHours(0, 0, 0, 0); // Normalize to start of the day

    if (selectedDate < today) {
      alert('You cannot search for trains on a past date.');
      return;
    }

    this.reservationService.getAvailableTrains(sourceStation, destinationStation, journeyDate).subscribe({
      next: (response) => {
        this.trains = response;
        this.searched = true;
        // console.log(this.trains);
      },
      error: (error) => {
        this.searched = true;
        console.error('Error fetching trains:', error);
        if (error.status === 401) {
          alert('Session expired. Please log in again.');
        }
        // if (error.status === 404) {
        //   alert('No Trains Available for selected route and date.');
        // }
      }
    });
  }

  fetchScheduleAndBook(trainID: number) {
    const journeyDate = this.searchForm.get('journeyDate')?.value;
    if (!journeyDate) {
      alert("Please select a journey date.");
      return;
    }

    this.scheduleService.getScheduleID(trainID, journeyDate).subscribe({
      next: (response) => {
        // console.log(trainID);
        console.log("Schedule Response:", response);

        if (response) {
          // console.log(`Redirecting to /book-ticket/${trainID}/${response.scheduleID}`);
          this.router.navigate(['/book-ticket', trainID, response.scheduleID]);
        } else {
          alert("No schedule found for the selected train and date.");
        }
      },
      error: (error) => {
        console.error("❌ Error fetching schedule ID:", error);
        alert("Failed to fetch schedule details. Please try again.");
      }
    });
  }

}
