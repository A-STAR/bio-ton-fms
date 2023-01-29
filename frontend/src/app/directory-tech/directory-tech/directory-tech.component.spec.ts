import { ComponentFixture, TestBed } from '@angular/core/testing';

import DirectoryTechComponent from './directory-tech.component';

describe('DirectoryTechComponent', () => {
  let component: DirectoryTechComponent;
  let fixture: ComponentFixture<DirectoryTechComponent>;

  beforeEach(async () => {
    await TestBed
      .configureTestingModule({
        imports: [DirectoryTechComponent]
      })
      .compileComponents();

    fixture = TestBed.createComponent(DirectoryTechComponent);
    component = fixture.componentInstance;

    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component)
      .toBeTruthy();
  });
});
