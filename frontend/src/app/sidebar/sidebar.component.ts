import { ChangeDetectionStrategy, Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { MatLegacyListModule as MatListModule } from '@angular/material/legacy-list';

import { firstValueFrom } from 'rxjs';

import { AuthService } from '../auth.service';

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

  /**
   * Handle navigation button action.
   *
   * @param type A type of button action.
   */
  async onNavigationButtonClick(type: NavigationButtonType) {
    switch (type) {
      case NavigationButtonType.SignOut:
        await firstValueFrom(this.authService.signOut$);

        await this.router.navigate(['/sign-in'], {
          replaceUrl: true
        });

        break;
    }
  }

  constructor(private router: Router, private authService: AuthService) { }
}

export enum NavigationButtonType {
  SignOut
}

type NavigationItem = {
  title: string;
  link?: string;
  icon: string;
  alt: string;
  type?: NavigationButtonType.SignOut;
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
        alt: 'Выход',
        type: NavigationButtonType.SignOut
      }
    ]
  ]
];
