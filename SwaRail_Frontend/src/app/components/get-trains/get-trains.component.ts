import { Component, OnInit } from '@angular/core';
import { TrainService } from '../../services/train.service';
import { Router } from '@angular/router';
import { FormBuilder, FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { TrainFilterPipe } from '../../pipes/train-filter.pipe';

@Component({
  selector: 'app-get-trains',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, TrainFilterPipe],
  templateUrl: './get-trains.component.html',
  styleUrl: './get-trains.component.css'
})
export class GetTrainsComponent implements OnInit {
  trains: any[] = [];
  searchControl = new FormControl<string | null>('');

  constructor(private trainService: TrainService, private router: Router) {}

  ngOnInit() {
    this.fetchAllTrains();
  }

  fetchAllTrains() {
    this.trainService.getAllTrains().subscribe({
      next: (data) => this.trains = data,
      error: (err) => console.error('Error fetching trains:', err)
    });
  }

  editTrain(trainId: number) {
    this.router.navigate(['/edit-train', trainId]);
  }

  cancelTrain(trainId: number) {
    if (confirm('Are you sure you want to cancel this train?')) {
      this.trainService.cancelTrain(trainId).subscribe({
        next: () => {
          alert(`Train with Id: ${trainId} cancelled successfully.`);
          this.fetchAllTrains(); // Refresh train list
        },
        error: (err) => console.error('Error cancelling train:', err)
      });
    }
  }

  addTrain() {
    this.router.navigate(['/add-train']);
  }
}