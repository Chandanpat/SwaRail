import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { TrainService } from '../../services/train.service';
import { ScheduleService } from '../../services/schedule.service';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';


@Component({
  selector: 'app-add-schedule',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './add-schedule.component.html',
  styleUrl: './add-schedule.component.css'
})
export class AddScheduleComponent implements OnInit {
  scheduleForm!: FormGroup;
  isMultiple: boolean = false;
  trains: any[] = [];

  constructor(
    private fb: FormBuilder,
    private scheduleService: ScheduleService,
    private trainService: TrainService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.initializeForm();
    this.loadTrains();
  }

  initializeForm() {
    this.scheduleForm = this.fb.group({
      trainID: ['', Validators.required],
      journeyDate: ['', Validators.required],
      toJourneyDate: [''] // Only used for multiple
    });
  }

  loadTrains() {
    this.trainService.getAllTrains().subscribe({
      next: res => this.trains = res,
      error: err => console.error("Failed to fetch trains", err)
    });
  }

  toggleScheduleMode(isMultiple: boolean) {
    this.isMultiple = isMultiple;
    this.scheduleForm.reset();
  }

  onSubmit() {
    if (this.scheduleForm.invalid) return;

    const formValue = this.scheduleForm.value;

    if (this.isMultiple) {
      const payload = {
        trainID: formValue.trainID,
        fromJourneyDate: formValue.journeyDate,
        toJourneyDate: formValue.toJourneyDate
      };
      this.scheduleService.addMultipleSchedules(payload).subscribe({
        next: () => {
          alert(`Schedule of Train Id: ${payload.trainID} has been added Successfully.`)
          this.router.navigate(['/get-schedule']);
        },
        error: err => alert("Failed to add multiple schedules: " + err.message)
      });
    } else {
      const payload = {
        trainID: formValue.trainID,
        journeyDate: formValue.journeyDate
      };
      this.scheduleService.addSingleSchedule(payload).subscribe({
        next: () => {
          alert(`Schedule of Train Id: ${payload.trainID} has been added Successfully.`)
          this.router.navigate(['/get-schedule']);
        },
        error: err => alert("Failed to add schedule: " + err.message)
      });
    }
  }
}
