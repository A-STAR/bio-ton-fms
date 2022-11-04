import { Routes } from '@angular/router';

export const directory: Routes = [
  {
    path: '',
    redirectTo: 'tech',
    pathMatch: 'full',
  },
  {
    path: 'tech',
    loadChildren: () => import('./directory-tech/directory-tech.routes')
      .then(({ directoryTech }) => directoryTech)
  }
];
