import { Routes } from '@angular/router';

const routes: Routes = [
  {
    path: '',
    redirectTo: 'tech',
    pathMatch: 'full',
  },
  {
    path: 'tech',
    loadChildren: () => import('./directory-tech/directory-tech.routes')
  }
];

export default routes;
