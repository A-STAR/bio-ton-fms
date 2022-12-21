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
        redirectTo: 'directory',
        pathMatch: 'full'
      },
      {
        path: 'sign-in',
        loadComponent: () => import('./sign-in/sign-in.component')
      },
      {
        path: 'directory',
        loadChildren: () => import('./directory.routes')
      }
    ]
  }
];
