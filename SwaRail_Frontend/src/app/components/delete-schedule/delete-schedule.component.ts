import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { TrainService } from '../../services/train.service';
import { ScheduleService } from '../../services/schedule.service';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-delete-schedule',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './delete-schedule.component.html',
  styleUrl: './delete-schedule.component.css'
})
export class DeleteScheduleComponent implements OnInit {
  deleteForm!: FormGroup;
  isMultiple: boolean = false;
  trains: any[] = [];

  constructor(
    private fb: FormBuilder,
    private scheduleService: ScheduleService,
    private trainService: TrainService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.deleteForm = this.fb.group({
      trainID: ['', Validators.required],
      journeyDate: ['', Validators.required],
      toJourneyDate: ['']
    });

    this.trainService.getAllTrains().subscribe({
      next: (data) => this.trains = data,
      error: (err) => console.error('Train fetch error:', err)
    });
  }

  toggleMode(multiple: boolean) {
    this.isMultiple = multiple;
    if (multiple) {
      this.deleteForm.addControl('toJourneyDate', this.fb.control('', Validators.required));
    } else {
      this.deleteForm.removeControl('toJourneyDate');
    }
  }

  onSubmit() {
    const trainID = this.deleteForm.value.trainID;
    const journeyDate = this.deleteForm.value.journeyDate;
    const toJourneyDate = this.deleteForm.value.toJourneyDate;

    if (this.isMultiple) {
      const body = {
        trainID,
        fromJourneyDate: journeyDate,
        toJourneyDate
      };
      this.scheduleService.deleteMultipleSchedules(body).subscribe({
        next: () => {
          alert(`Schedule of Train Id: ${trainID} has been deleted Successfully.`)
          this.router.navigate(['/get-schedule']);
        },
        error: (err) => alert('Delete failed: ' + err.error?.error || err.message)
      });
    } else {
      const body = {
        trainID,
        journeyDate
      };
      this.scheduleService.deleteSingleSchedule(body).subscribe({
        next: () => {
          alert(`Schedule of Train Id: ${trainID} has been deleted Successfully.`)
          this.router.navigate(['/get-schedule']);
        },
        error: (err) => alert('Delete failed: ' + err.error?.error || err.message)
      });
    }
  }
}
