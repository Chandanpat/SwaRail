import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ReservationService } from '../../services/reservation.service';
import { CommonModule, DatePipe } from '@angular/common';
import { TrainService } from '../../services/train.service';
import { ScheduleService } from '../../services/schedule.service';

@Component({
  selector: 'app-book-ticket',
  standalone: true,
  imports: [FormsModule, CommonModule, ReactiveFormsModule],
  templateUrl: './book-ticket.component.html',
  styleUrl: './book-ticket.component.css',
  providers: [DatePipe] 
})
export class BookTicketComponent implements OnInit {
  bookForm: FormGroup;
  trainDetails: any = null;
  scheduleDetails: any = null;
  maxPassengers = 6;
  trainID: number;
  scheduleID: number;
  journeyDate: string = '';

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private datePipe: DatePipe,
    private reservationService: ReservationService,
    private trainService: TrainService,
    private scheduleService: ScheduleService
  ) {
    this.bookForm = this.fb.group({
      coachType: ['Sleeper', Validators.required],
      passengers: this.fb.array([this.createPassenger()])
    });

    this.trainID = +this.route.snapshot.paramMap.get('trainID')!;
    this.scheduleID = +this.route.snapshot.paramMap.get('scheduleID')!;
  }

  ngOnInit() {
    this.fetchTrainDetails();
    this.fetchScheduleDetails();
  }

  fetchTrainDetails() {
    this.trainService.getTrainById(this.trainID).subscribe({
      next: (response) => {
        this.trainDetails = response;
      },
      error: (error) => {
        console.error('Error fetching train details:', error);
      }
    });
  }

  fetchScheduleDetails() {
    this.scheduleService.getScheduleById(this.scheduleID).subscribe({
      next: (response) => {
        this.scheduleDetails = response;
        this.journeyDate = this.datePipe.transform(response.journeyDate, 'dd-MMM-yyyy') || '';
      },
      error: (error) => {
        console.error('Error fetching schedule details:', error);
      }
    });
  }

  get passengers() {
    return this.bookForm.get('passengers') as FormArray;
  }

  createPassenger(): FormGroup {
    return this.fb.group({
      name: ['', Validators.required],
      age: ['', [Validators.required, Validators.min(1)]],
      sex: ['', Validators.required],
      address: ['', Validators.required]
    });
  }

  addPassenger() {
    if (this.passengers.length < this.maxPassengers) {
      this.passengers.push(this.createPassenger());
    } else {
      alert('You can only add up to 6 passengers.');
    }
  }

  removePassenger(index: number) {
    if (this.passengers.length > 1) {
      this.passengers.removeAt(index);
    }
  }

  bookTicket() {
    if (this.bookForm.invalid) {
      alert('Please fill all required fields.');
      return;
    }

    const request = {
      trainID: this.trainID,
      scheduleID: this.scheduleID,
      coachType: this.bookForm.value.coachType,
      passengers: this.bookForm.value.passengers
    };

    this.reservationService.bookTicket(request).subscribe({
      next: (response) => {
        console.log(response);
        alert(response.message);
        this.router.navigate(['/make-payment', response.reservationID]);
      },
      error: (error) => {
        console.error('Error booking ticket:', error);
        alert('Failed to book ticket. Please try again.');
      }
    });
  }
}
