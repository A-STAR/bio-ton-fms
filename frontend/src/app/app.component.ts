import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';

import { Observable } from 'rxjs';

import { AuthService } from './auth.service';

@Component({
  selector: 'bio-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.sass'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AppComponent implements OnInit {
  authenticated$!: Observable<boolean>;

  constructor(private authService: AuthService) { }

  ngOnInit() {
    this.authenticated$ = this.authService.authenticated$;
  }
}
