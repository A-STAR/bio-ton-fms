import { ChangeDetectionStrategy, Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'bio-directory-tech',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatButtonModule
  ],
  templateUrl: './directory-tech.component.html',
  styleUrls: ['./directory-tech.component.sass'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export default class DirectoryTechComponent {
  protected get navigation() {
    return navigation;
  }
}

type NavigationItem = {
  title: string;
  link: string;
}

export const navigation: NavigationItem[] = [
  {
    title: 'Машины',
    link: 'vehicles'
  },
  {
    title: 'GPS-трекеры',
    link: 'trackers'
  }
];
