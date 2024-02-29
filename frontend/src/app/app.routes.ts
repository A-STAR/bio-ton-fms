import { Routes } from '@angular/router';

import { AuthGuard } from './auth.guard';

export const routes: Routes = [
  {
    path: '',
    canActivate: [AuthGuard],
    canLoad: [AuthGuard],
    children: [
      {
        path: '',
        redirectTo: 'messages',
        pathMatch: 'full'
      },
      {
        path: 'sign-in',
        loadComponent: () => import('./sign-in/sign-in.component')
      },
      {
        path: 'agro',
        loadComponent: () => import('./agro/agro.component')
      },
      {
        path: 'tech',
        loadComponent: () => import('./tech/tech.component')
      },
      {
        path: 'operations',
        loadComponent: () => import('./shared/mock/mock.component'),
        data: {
          title: 'Агрооперации'
        }
      },
      {
        path: 'tasks',
        loadComponent: () => import('./shared/mock/mock.component'),
        data: {
          title: 'Задания Машин'
        }
      },
      {
        path: 'notifications',
        loadComponent: () => import('./shared/mock/mock.component'),
        data: {
          title: 'Уведомления'
        }
      },
      {
        path: 'messages',
        loadComponent: () => import('./messages/messages.component')
      },
      {
        path: 'directory',
        loadChildren: () => import('./directory.routes')
      }
    ]
  }
];
