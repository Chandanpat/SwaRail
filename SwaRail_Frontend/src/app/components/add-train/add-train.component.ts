import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { TrainService } from '../../services/train.service';
import { Router, RouterModule } from '@angular/router';
import { StationService } from '../../services/station.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-add-train',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './add-train.component.html',
  styleUrl: './add-train.component.css'
})
export class AddTrainComponent {
  addTrainForm: FormGroup;
  stations: any[] = [];

  constructor(
    private fb: FormBuilder,
    private trainService: TrainService,
    private stationService: StationService,
    private router: Router
  ) {
    this.addTrainForm = this.fb.group({
      trainName: ['', Validators.required],
      sourceID: ['', Validators.required],
      destinationID: ['', Validators.required],
      totalSeats: ['', [Validators.required, Validators.min(1)]],
      fare: ['', [Validators.required, Validators.min(0)]],
      sourceDepartureTime: ['', Validators.required],
      destiArrivalTime: ['', Validators.required]
    });

    this.loadStations();
  }

  loadStations() {
    this.stationService.getStations().subscribe({
      next: (res) => this.stations = res,
      error: (err) => console.error('Error fetching stations', err)
    });
  }

  addTrain() {
    if (this.addTrainForm.valid) {
      const trainData = this.addTrainForm.value;
      this.trainService.addTrain(trainData).subscribe({
        next: (res) => {
          console.log(res);
          this.router.navigate(['/get-trains']);
        },
        error: (err) => console.error('Train add failed', err)
      });
    }
  }
}
