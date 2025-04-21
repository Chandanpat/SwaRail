import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ScheduleService } from '../../services/schedule.service';
import { TrainService } from '../../services/train.service';
import { CommonModule } from '@angular/common';
import { ScheduleFilterPipe } from '../../pipes/schedule-filter.pipe';
import { Router } from '@angular/router';

@Component({
  selector: 'app-get-schedule',
  standalone: true,
  imports: [FormsModule, ReactiveFormsModule,CommonModule, ScheduleFilterPipe],
  templateUrl: './get-schedule.component.html',
  styleUrl: './get-schedule.component.css'
})
export class GetScheduleComponent  implements OnInit {
  schedules: any[] = [];
  trains: any[] = [];
  searchForm!: FormGroup;

  constructor(
    private scheduleService: ScheduleService,
    private trainService: TrainService,
    private fb: FormBuilder,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadSchedules();
    this.loadTrains();

    this.searchForm = this.fb.group({
      scheduleID: [''],
      trainName: [''],
      journeyDate: ['']
    });
  }

  loadSchedules(): void {
    this.scheduleService.getAllSchedules().subscribe({
      next: (data) => (this.schedules = data),
      error: (err) => console.error('Error loading schedules:', err)
    });
  }

  loadTrains(): void {
    this.trainService.getAllTrains().subscribe({
      next: (data) => (this.trains = data),
      error: (err) => console.error('Error loading trains:', err)
    });
  }

  getTrainName(trainId: number): string {
    const train = this.trains.find(t => t.trainID === trainId);
    return train ? train.trainName : 'Unknown Train';
  }

  deleteSchedule(trainId: number, journeyDate: string): void {
    const body = {
      trainId,
      journeyDate
    };
    this.scheduleService.deleteSingleSchedule(body).subscribe({
      next: () => {
        alert(`Schedule of Train Id: ${trainId} has been deleted Successfully.`);
        this.loadSchedules();
      },
      error: (err) => console.error('Error deleting schedule:', err)
    });
  }

  get searchValues() {
    return this.searchForm.value;
  }

  addSchedule() {
    this.router.navigate(['/add-schedule']);
  }

  deleteScheduleBtn() {
    this.router.navigate(['/delete-schedule']);
  }
}