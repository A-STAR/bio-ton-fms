import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';

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
  protected vehiclesData$!: Observable<[Vehicles, VehicleGroup[], Fuel[]]>;
  #vehicles$ = new BehaviorSubject(undefined);

  /**
   * Get vehicles, groups, fuels. Set vehicles data.
   */
  #setVehiclesData() {
    this.vehiclesData$ = this.#vehicles$.pipe(
      mergeMap(() => forkJoin([
        this.vehiclesService.getVehicles(),
        this.vehiclesService.vehicleGroups$,
        this.vehiclesService.fuels$
      ]))
    );
  }

  constructor(private vehiclesService: VehicleService) { }

  ngOnInit() {
    this.#setVehiclesData();
  }
}
