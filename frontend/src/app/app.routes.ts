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
        redirectTo: 'tech',
        pathMatch: 'full'
      },
      {
        path: 'sign-in',
        loadComponent: () => import('./sign-in/sign-in.component')
      },
      {
        path: 'tech',
        loadComponent: () => import('./tech/tech.component')
      },
      {
        path: 'directory',
        loadChildren: () => import('./directory.routes')
      }
    ]
  }
];
