import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ReactiveFormsModule, ValidationErrors, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatAutocompleteModule } from '@angular/material/autocomplete';

import { Observable, debounceTime, defer, distinctUntilChanged, filter, map, skipWhile, startWith, switchMap } from 'rxjs';

import { MessageService } from './message.service';

import { MapComponent } from '../shared/map/map.component';

import { DEBOUNCE_DUE_TIME, MonitoringTech, SEARCH_MIN_LENGTH } from '../tech/tech.component';

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
  /**
   * Get search stream.
   *
   * @returns An `Observable` of search stream.
   */
  get #search$() {
    return this.selectionForm
      .get('tech')!
      .valueChanges.pipe(
        filter(value => typeof value !== 'object'),
        map(value => value as string | undefined),
        debounceTime(DEBOUNCE_DUE_TIME),
        map(searchValue => searchValue
          ?.trim()
          ?.toLocaleLowerCase()
        ),
        distinctUntilChanged(),
        skipWhile(searchValue => searchValue ? searchValue.length < SEARCH_MIN_LENGTH : true),
        map(searchValue => searchValue !== undefined && searchValue.length < SEARCH_MIN_LENGTH ? undefined : searchValue),
        distinctUntilChanged()
      );
  }

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

  #selectionRequiredValidator({ value }: AbstractControl): ValidationErrors | null {
    if (value && typeof value !== 'object') {
      return {
        selectionRequired: true
      };
    }

    return null;
  }

  /**
   * Initialize Selection form.
   */
  #initSelectionForm() {
    this.selectionForm = this.fb.group({
      tech: this.fb.nonNullable.control<MonitoringTech | string | undefined>(undefined, [
        Validators.required,
        this.#selectionRequiredValidator
      ])
    });
  }

  /**
   * Set tech.
   */
  #setTech() {
    const tech$ = defer(
      () => this.messageService.getVehicles()
    );

    const searchTech = (findCriterion: string) => this.messageService.getVehicles({ findCriterion });

    this.tech$ = this.#search$.pipe(
      startWith(undefined),
      switchMap(searchQuery => searchQuery ? searchTech(searchQuery) : tech$)
    );
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
