import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { StationService } from '../../services/station.service';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { StationFilterPipe } from '../../pipes/station-filter.pipe';

@Component({
  selector: 'app-manage-stations',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule, FormsModule, StationFilterPipe],
  templateUrl: './manage-stations.component.html',
  styleUrl: './manage-stations.component.css'
})
export class ManageStationsComponent implements OnInit {
  stationForm!: FormGroup;
  stations: any[] = [];
  searchText: string = '';
  isEdit: boolean = false;
  editingStationId: number | null = null;

  constructor(
    private fb: FormBuilder,
    private stationService: StationService
  ) {}

  ngOnInit(): void {
    this.stationForm = this.fb.group({
      stationName: ['', Validators.required]
    });

    this.loadStations();
  }

  loadStations() {
    this.stationService.getStations().subscribe({
      next: res => this.stations = res,
      error: err => console.error('Error fetching stations:', err)
    });
  }

  onSubmit() {
    if (this.stationForm.invalid) return;

    const stationData = this.stationForm.value;

    if (this.isEdit && this.editingStationId !== null) {
      this.stationService.updateStation(this.editingStationId, stationData).subscribe({
        next: () => {
          this.resetForm();
          this.loadStations();
        },
        error: err => console.error('Update failed:', err)
      });
    } else {
      this.stationService.addStation(stationData).subscribe({
        next: () => {
          this.resetForm();
          this.loadStations();
        },
        error: err => console.error('Add failed:', err)
      });
    }
  }

  onEdit(station: any) {
    this.stationForm.patchValue({
      stationName: station.stationName
    });
    this.isEdit = true;
    this.editingStationId = station.stationID;
  }

  onDelete(stationId: number) {
    if (confirm('Are you sure you want to delete this station?')) {
      this.stationService.deleteStation(stationId).subscribe({
        next: () => this.loadStations(),
        error: err => console.error('Delete failed:', err)
      });
    }
  }

  resetForm() {
    this.stationForm.reset();
    this.isEdit = false;
    this.editingStationId = null;
  }
}