import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';

import { forkJoin, map, Observable } from 'rxjs';

import { Fuel, VehicleGroup, VehicleService } from '../vehicle.service';

@Component({
  selector: 'bio-vehicle-dialog',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './vehicle-dialog.component.html',
  styleUrls: ['./vehicle-dialog.component.sass'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class VehicleDialogComponent implements OnInit {
  /**
   * Get groups, fuels. Set vehicle data.
   */
  #setVehicleData() {
    this.vehicleData$ = forkJoin([this.vehicleService.vehicleGroups$, this.vehicleService.fuels$])
      .pipe(
        map(([groups, fuels]) => ({ groups, fuels }))
      );
  }

  protected vehicleData$!: Observable<{
    groups: VehicleGroup[];
    fuels: Fuel[];
  }>;

  constructor(private vehicleService: VehicleService) { }

  ngOnInit() {
    this.#setVehicleData();
  }
}
