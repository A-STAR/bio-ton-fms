import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

import { Observable } from 'rxjs';

import { AuthService } from './auth.service';

import { SidebarComponent } from './sidebar/sidebar.component';

@Component({
  selector: 'bio-root',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    SidebarComponent
  ],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.sass'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AppComponent implements OnInit {
  authenticated$!: Observable<boolean>;

  constructor(private authService: AuthService) { }

  // eslint-disable-next-line @typescript-eslint/member-ordering
  ngOnInit() {
    this.authenticated$ = this.authService.authenticated$;
  }
}
