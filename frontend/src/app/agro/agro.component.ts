import { ChangeDetectionStrategy, Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'bio-agro',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './agro.component.html',
  styleUrls: ['./agro.component.sass'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export default class AgroComponent { }
