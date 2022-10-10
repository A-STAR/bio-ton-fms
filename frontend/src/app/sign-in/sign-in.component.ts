import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatDividerModule } from '@angular/material/divider';

import { Observable } from 'rxjs';

import { SystemService } from '../system.service';

@Component({
  selector: 'bio-sign-in',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatCardModule,
    MatDividerModule
  ],
  templateUrl: './sign-in.component.html',
  styleUrls: ['./sign-in.component.sass'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class SignInComponent implements OnInit {
  protected systemVersion$!: Observable<string>;

  constructor(private systemService: SystemService) { }

  ngOnInit() {
    this.systemVersion$ = this.systemService.getVersion$;
  }
}
