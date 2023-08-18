import { LOCALE_ID } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DATE_PIPE_DEFAULT_OPTIONS, formatDate, formatNumber, registerLocaleData } from '@angular/common';
import localeRu from '@angular/common/locales/ru';

import { RelativeTimePipe, localeID } from '../../relative-time.pipe';

import { TechMonitoringInfoComponent } from './tech-monitoring-info.component';

import { dateFormat } from '../../../../directory-tech/trackers/trackers.component.spec';
import { testVehicleMonitoringInfo } from '../../../../../app/tech/tech.service.spec';

describe('TechMonitoringInfoComponent', () => {
  let component: TechMonitoringInfoComponent;
  let fixture: ComponentFixture<TechMonitoringInfoComponent>;

  beforeEach(async () => {
    await TestBed
      .configureTestingModule({
        imports: [TechMonitoringInfoComponent],
        providers: [
          {
            provide: LOCALE_ID,
            useValue: localeID
          },
          {
            provide: DATE_PIPE_DEFAULT_OPTIONS,
            useValue: { dateFormat }
          },
          RelativeTimePipe
        ]
      })
      .compileComponents();

    registerLocaleData(localeRu, localeID);

    fixture = TestBed.createComponent(TechMonitoringInfoComponent);

    component = fixture.componentInstance;
    component.info = testVehicleMonitoringInfo;

    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component)
      .toBeTruthy();
  });

  it('should render basic info', () => {
    const relativeTimePipe = TestBed.inject(RelativeTimePipe);

    const descriptionListDe = fixture.debugElement.query(
      By.css('dl')
    );

    const descriptionTermDes = descriptionListDe.queryAll(
      By.css('dt')
    );

    const descriptionDetailsDes = descriptionListDe.queryAll(
      By.css('dd')
    );

    const { lastMessageTime, speed, mileage, engineHours, satellitesNumber, latitude, longitude } = testVehicleMonitoringInfo.generalInfo;

    const DESCRIPTION_TEXTS = [
      {
        term: 'Последняя точка:',
        details: [
          relativeTimePipe.transform(lastMessageTime),
          formatDate(lastMessageTime!, dateFormat, localeID)
        ]
      },
      {
        term: 'Скорость',
        details: [`${formatNumber(speed!, 'en-US', '1.1-1')} км/ч`]
      },
      {
        term: 'Пробег',
        details: [`${formatNumber(mileage!, localeID, '1.0-0')} км`]
      },
      {
        term: 'Моточасы',
        details: [`${engineHours} ч`]
      },
      {
        term: 'Спутники',
        details: [
          satellitesNumber?.toString()
        ]
      },
      {
        term: 'Координаты',
        details: [`ш: ${formatNumber(latitude!, 'en-US', '1.6-6')}°д: ${formatNumber(longitude!, 'en-US', '1.6-6')}°`]
      }
    ];

    let descriptionDetailsDeIndex = 0;

    descriptionTermDes.forEach((descriptionTermDe, index) => {
      expect(descriptionTermDe.nativeElement.textContent)
        .withContext('render description term text')
        .toBe(DESCRIPTION_TEXTS[index].term);

      DESCRIPTION_TEXTS[index].details.forEach(detailsText => {
        expect(descriptionDetailsDes[descriptionDetailsDeIndex].nativeElement.textContent)
          .withContext('render description details text')
          .toBe(detailsText);

        descriptionDetailsDeIndex += 1;
      });
    });
  });

  it('should render sensors info', () => {
    const headingDe = fixture.debugElement.query(
      By.css('h1')
    );

    expect(headingDe)
      .withContext('render heading element')
      .not.toBeNull();

    expect(headingDe.nativeElement.textContent)
      .withContext('render heading text')
      .toBe('Значения датчиков');

    const descriptionListDe = fixture.debugElement.query(
      By.css('dl:nth-of-type(2)')
    );

    const descriptionTermDes = descriptionListDe.queryAll(
      By.css('dt')
    );

    const descriptionDetailsDes = descriptionListDe.queryAll(
      By.css('dd')
    );

    const { sensors } = testVehicleMonitoringInfo.trackerInfo;

    descriptionTermDes.forEach((descriptionTermDe, index) => {
      const { name, value, unit } = sensors![index];

      expect(descriptionTermDe.nativeElement.textContent)
        .withContext('render description term text')
        .toBe(`${name}:`);

      const expectedDetailsText = value ? `${value} ${unit}` : '';

      expect(descriptionDetailsDes[index].nativeElement.textContent)
        .withContext('render description details text')
        .toBe(expectedDetailsText);
    });
  });
});
