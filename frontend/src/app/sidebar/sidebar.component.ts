import { ChangeDetectionStrategy, Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatListModule } from '@angular/material/list';

@Component({
  selector: 'bio-sidebar',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatListModule
  ],
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.sass'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class SidebarComponent {
  protected get NAVIGATION(): NavigationItem[][][] {
    return NAVIGATION;
  }
}

type NavigationItem = {
  title: string;
  link?: string;
  icon: string;
  alt: string;
};

export const NAVIGATION: NavigationItem[][][] = [
  [
    [
      {
        title: 'Мониторинг Агро',
        link: '/agro',
        icon: 'agro',
        alt: 'Мониторинг агро'
      },
      {
        title: 'Мониторинг Техника',
        link: '/tech',
        icon: 'tech',
        alt: 'Мониторинг техника'
      }
    ],
    [
      {
        title: 'Агрооперации',
        link: '/operations',
        icon: 'operations',
        alt: 'Агрооперации'
      },
      {
        title: 'Задания Машин',
        link: '/tasks',
        icon: 'tasks',
        alt: 'Задания машин'
      }
    ],
    [
      {
        title: 'Уведомления',
        link: '/notifications',
        icon: 'notifications',
        alt: 'Уведомления'
      },
      {
        title: 'Сообщения',
        link: '/messages',
        icon: 'messages',
        alt: 'Сообщения'
      }
    ],
    [
      {
        title: 'Отчеты',
        link: '/reports',
        icon: 'reports',
        alt: 'Отчеты'
      },
      {
        title: 'Пользователи',
        link: '/users',
        icon: 'users',
        alt: 'Пользователи'
      },
      {
        title: 'Справочники',
        link: '/directory',
        icon: 'directory',
        alt: 'Справочники'
      }
    ]
  ],
  [
    [
      {
        title: 'Аккаунт',
        link: '/account',
        icon: 'account',
        alt: 'Аккаунт'
      },
      {
        title: 'Настройки',
        link: '/settings',
        icon: 'settings',
        alt: 'Настройки'
      },
      {
        title: 'Выход',
        icon: 'exit',
        alt: 'Выход'
      }
    ]
  ]
];
