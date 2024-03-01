import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { ActivatedRoute, Data } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';

import { of } from 'rxjs';

import MockComponent, { DEFAULT_MOCK } from './mock.component';

describe('MockComponent', () => {
  let component: MockComponent;
  let fixture: ComponentFixture<MockComponent>;

  beforeEach(async () => {
    await TestBed
      .configureTestingModule({
        imports: [
          RouterTestingModule,
          MockComponent
        ],
        providers: [
          {
            provide: ActivatedRoute,
            useValue: testActivatedRoute
          }
        ]
      })
      .compileComponents();

    fixture = TestBed.createComponent(MockComponent);

    component = fixture.componentInstance;

    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component)
      .toBeTruthy();
  });

  it('should render mock image', () => {
    const mockImageDe = fixture.debugElement.query(
      By.css('img')
    );

    expect(mockImageDe)
      .withContext('render mock image element')
      .not.toBeNull();

    expect(mockImageDe.nativeElement.src)
      .withContext('render mock image source')
      .toMatch(`${testData['mock']}$`);

    expect(mockImageDe.nativeElement.alt)
      .withContext('render mock image alternate text')
      .toBe(testData['title']);
  });
});

describe('MockComponent without mock', () => {
  let component: MockComponent;
  let fixture: ComponentFixture<MockComponent>;

  beforeEach(async () => {
    await TestBed
      .configureTestingModule({
        imports: [
          RouterTestingModule,
          MockComponent
        ],
        providers: [
          {
            provide: ActivatedRoute,
            useValue: {
              data: of({
                title: testData['title']
              }),
              snapshot: {
                data: {
                  title: testData['title']
                }
              }
            }
          }
        ]
      })
      .compileComponents();

    fixture = TestBed.createComponent(MockComponent);

    component = fixture.componentInstance;

    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component)
      .toBeTruthy();
  });

  it('should render default mock image', () => {
    const mockImageDe = fixture.debugElement.query(
      By.css('img')
    );

    expect(mockImageDe)
      .withContext('render mock image element')
      .not.toBeNull();

    expect(mockImageDe.nativeElement.src)
      .withContext('render mock image source')
      .toMatch(DEFAULT_MOCK);

    expect(mockImageDe.nativeElement.alt)
      .withContext('render mock image alternate text')
      .toBe(testData['title']);
  });
});

const testData: Data = {
  title: 'Mock',
  mock: 'assets/images/test.png'
};

const testActivatedRoute = {
  data: of(testData),
  snapshot: {
    data: testData
  }
} as Pick<ActivatedRoute, 'data' | 'snapshot'>;
