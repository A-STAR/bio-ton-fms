import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { RouterTestingModule } from '@angular/router/testing';

import { firstValueFrom } from 'rxjs';

import { SidebarComponent } from './sidebar/sidebar.component';

import { AuthService } from './auth.service';

import { AppComponent } from './app.component';

describe('AppComponent', () => {
  let app: AppComponent;
  let fixture: ComponentFixture<AppComponent>;
  let authService: AuthService;

  beforeEach(async () => {
    await TestBed
      .configureTestingModule({
        declarations: [AppComponent],
        imports: [
          RouterTestingModule,
          SidebarComponent
        ]
      })
      .compileComponents();

    fixture = TestBed.createComponent(AppComponent);
    authService = TestBed.inject(AuthService);

    app = fixture.componentInstance;

    fixture.detectChanges();
  });

  it('should create the app', () => {
    expect(app)
      .toBeTruthy();
  });

  it('should set authenticated state', async () => {
    await expectAsync(firstValueFrom(app.authenticated$))
      .withContext('initialize `autnenticated$`')
      .toBeResolvedTo(false);

    authService.setAuthenticated(true);

    await expectAsync(firstValueFrom(app.authenticated$))
      .withContext('set `autnenticated$`')
      .toBeResolvedTo(true);
  });

  it('should render sidebar', () => {
    let sidebar = fixture.debugElement.query(By.css('bio-sidebar'));

    expect(sidebar)
      .withContext('hide sidebar in unauthenticated state')
      .toBeNull();

    authService.setAuthenticated(true);
    fixture.detectChanges();

    sidebar = fixture.debugElement.query(By.css('bio-sidebar'));

    expect(sidebar)
      .withContext('render sidebar in authenticated state')
      .not.toBeNull();
  });
});
