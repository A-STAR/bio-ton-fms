import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatAutocompleteModule } from '@angular/material/autocomplete';

import { Observable } from 'rxjs';

import { MessageService } from './message.service';

import { MapComponent } from '../shared/map/map.component';

import { MonitoringTech } from '../tech/tech.component';

@Component({
  selector: 'bio-messages',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatAutocompleteModule,
    MapComponent
  ],
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.sass'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export default class MessagesComponent implements OnInit {
  protected selectionForm!: MessageSelectionForm;
  protected tech$!: Observable<MonitoringTech[]>;

  /**
   * Map a tech option's control value to its name display value in the trigger.
   *
   * @param tech `MonitoringTech` tech.
   *
   * @returns `MonitoringTech` display value.
   */
  protected displayFn(tech: MonitoringTech): string {
    return tech?.name ?? '';
  }

  /**
   * `TrackByFunction` to compute the identity of tech.
   *
   * @param index The index of the item within the iterable.
   * @param tech The `MonitoringTech` in the iterable.
   *
   * @returns `MonitoringTech` ID.
   */
  protected techTrackBy(index: number, { id }: MonitoringTech) {
    return id;
  }

  /**
   * Initialize Selection form.
   */
  #initSelectionForm() {
    this.selectionForm = this.fb.group({
      tech: this.fb.nonNullable.control<MonitoringTech | string | undefined>(undefined)
    });
  }

  /**
   * Set tech.
   */
  #setTech() {
    this.tech$ = this.messageService.getVehicles();
  }

  constructor(private fb: FormBuilder, private messageService: MessageService) { }

  // eslint-disable-next-line @typescript-eslint/member-ordering
  ngOnInit() {
    this.#initSelectionForm();
    this.#setTech();
  }
}

type MessageSelectionForm = FormGroup<{
  tech: FormControl<MonitoringTech | string | undefined>;
}>;
