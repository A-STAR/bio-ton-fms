import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { CommonModule, KeyValue } from '@angular/common';

import { BehaviorSubject, forkJoin, mergeMap, Observable } from 'rxjs';

import { Fuel, VehicleGroup, Vehicles, VehicleService } from '../vehicle.service';

@Component({
  selector: 'bio-vehicles',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './vehicles.component.html',
  styleUrls: ['./vehicles.component.sass'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class VehiclesComponent implements OnInit {
  protected vehiclesData$!: Observable<[Vehicles, VehicleGroup[], Fuel[], KeyValue<string, string>[], KeyValue<string, string>[]]>;
  #vehicles$ = new BehaviorSubject(undefined);

  /**
   * Get vehicles, groups, fuels, type, subtype. Set vehicles data.
   */
  #setVehiclesData() {
    this.vehiclesData$ = this.#vehicles$.pipe(
      mergeMap(() => forkJoin([
        this.vehiclesService.getVehicles(),
        this.vehiclesService.vehicleGroups$,
        this.vehiclesService.fuels$,
        this.vehiclesService.vehicleType$,
        this.vehiclesService.vehicleSubType$
      ]))
    );
  }

  constructor(private vehiclesService: VehicleService) { }

  ngOnInit() {
    this.#setVehiclesData();
  }
}
