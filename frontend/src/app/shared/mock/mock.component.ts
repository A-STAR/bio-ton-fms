import { ChangeDetectionStrategy, Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'bio-mock',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './mock.component.html',
  styleUrls: ['./mock.component.sass'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export default class MockComponent { }
