import { ChangeDetectionStrategy, Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'bio-tech',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './tech.component.html',
  styleUrls: ['./tech.component.sass'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export default class TechComponent { }
