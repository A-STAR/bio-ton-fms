import { ChangeDetectionStrategy, Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'bio-directory-tech',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule
  ],
  templateUrl: './directory-tech.component.html',
  styleUrls: ['./directory-tech.component.sass'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export default class DirectoryTechComponent { }
