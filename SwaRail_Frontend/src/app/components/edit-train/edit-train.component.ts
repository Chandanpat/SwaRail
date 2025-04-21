import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { TrainService } from '../../services/train.service';
import { StationService } from '../../services/station.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-edit-train',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './edit-train.component.html',
  styleUrl: './edit-train.component.css'
})
export class EditTrainComponent implements OnInit {
  editTrainForm!: FormGroup;
  stations: any[] = [];
  trainId!: number;

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private trainService: TrainService,
    private stationService: StationService
  ) {
    this.editTrainForm = this.fb.group({
      trainName: ['', Validators.required],
      sourceID: ['', Validators.required],
      destinationID: ['', Validators.required],
      totalSeats: ['', [Validators.required, Validators.min(1)]],
      fare: ['', [Validators.required, Validators.min(0)]],
      sourceDepartureTime: ['', Validators.required],
      destiArrivalTime: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    this.trainId = Number(this.route.snapshot.paramMap.get('trainId'));
    this.loadStations();
    this.loadTrainData();
  }

  loadStations() {
    this.stationService.getStations().subscribe({
      next: (res) => this.stations = res,
      error: (err) => console.error('Error fetching stations', err)
    });
  }

  loadTrainData() {
    this.trainService.getTrainById(this.trainId).subscribe({
      next: (train) => {
        this.stationService.getStationByName(train.sourceStation).subscribe({
          next: (sourceStationData) => {
            this.stationService.getStationByName(train.destinationStation).subscribe({
              next: (destinationStationData) => {
                this.editTrainForm.patchValue({
                  trainName: train.trainName,
                  sourceID: sourceStationData.stationID,
                  destinationID: destinationStationData.stationID,
                  totalSeats: train.totalSeats,
                  fare: train.fare,
                  sourceDepartureTime: train.sourceDepartureTime,
                  destiArrivalTime: train.destiArrivalTime
                });
              },
              error: (err) => console.error('Error loading Destination Station', err)
            });
          },
          error: (err) => console.error('Error loading Source Station', err)
        });
      },
      error: (err) => console.error('Error loading train data', err)
    });
  }

  updateTrain() {
    if (this.editTrainForm.valid) {
      this.trainService.updateTrain(this.trainId, this.editTrainForm.value).subscribe({
        next: (res) => {
          console.log(res);
          this.router.navigate(['/get-trains']);
        },
        error: (err) => console.error('Error updating train', err)
      });
    }
  }
}