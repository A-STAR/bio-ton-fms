import { ChangeDetectionStrategy, Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'bio-vehicles',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './vehicles.component.html',
  styleUrls: ['./vehicles.component.sass'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class VehiclesComponent { }
