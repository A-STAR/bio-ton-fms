import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'bio-mock',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './mock.component.html',
  styleUrls: ['./mock.component.sass'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export default class MockComponent implements OnInit {
  protected mock!: PageMock;

  constructor(private route: ActivatedRoute) { }

  // eslint-disable-next-line @typescript-eslint/member-ordering
  ngOnInit() {
    this.mock = {
      title: this.route.snapshot.data['title'],
      path: this.route.snapshot.data['mock'] ?? DEFAULT_MOCK
    };
  }
}

type PageMock = {
  title: string;
  path: string;
};

export const DEFAULT_MOCK = 'assets/images/mock.png';
