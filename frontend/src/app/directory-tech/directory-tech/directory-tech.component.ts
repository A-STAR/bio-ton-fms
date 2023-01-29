import { ChangeDetectionStrategy, Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'bio-directory-tech',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './directory-tech.component.html',
  styleUrls: ['./directory-tech.component.sass'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export default class DirectoryTechComponent { }
