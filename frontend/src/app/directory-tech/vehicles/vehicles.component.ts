import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';

import { BehaviorSubject, mergeMap, Observable } from 'rxjs';

import { Vehicles, VehicleService } from '../vehicle.service';

@Component({
  selector: 'bio-vehicles',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './vehicles.component.html',
  styleUrls: ['./vehicles.component.sass'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class VehiclesComponent implements OnInit {
  protected vehicles$!: Observable<Vehicles>;
  #vehicles$ = new BehaviorSubject(undefined);

  /**
   * Get and set vehicles.
   */
  #setVehicles() {
    this.vehicles$ = this.#vehicles$.pipe(
      mergeMap(() => this.vehiclesService.getVehicles())
    );
  }

  constructor(private vehiclesService: VehicleService) { }

  ngOnInit() {
    this.#setVehicles();
  }
}
