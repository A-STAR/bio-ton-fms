import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { CommonModule, KeyValue } from '@angular/common';

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
   * Get groups, fuels, type, subtype. Set vehicle data.
   */
  #setVehicleData() {
    this.vehicleData$ = forkJoin([
      this.vehicleService.vehicleGroups$,
      this.vehicleService.fuels$,
      this.vehicleService.vehicleType$,
      this.vehicleService.vehicleSubtype$
    ])
      .pipe(
        map(([groups, fuels, type, subtype]) => ({ groups, fuels, type, subtype }))
      );
  }

  protected vehicleData$!: Observable<{
    groups: VehicleGroup[];
    fuels: Fuel[];
    type: KeyValue<string, string>[];
    subtype: KeyValue<string, string>[];
  }>;

  constructor(private vehicleService: VehicleService) { }

  ngOnInit() {
    this.#setVehicleData();
  }
}
