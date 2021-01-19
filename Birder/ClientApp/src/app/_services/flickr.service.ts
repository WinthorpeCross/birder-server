import { Observable, of } from 'rxjs';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'environments/environment';
import { FlickrUrlsViewModel } from '../_models/FlickrUrlsViewModel';
import { TokenService } from './token.service';
import { tokenGetter } from '@app/app.module';

@Injectable({
  providedIn: 'root'
})
export class FlickrService {
  private readonly apiKey = this.token.getFlikrKey();
  private readonly apiUrl = 'https://api.flickr.com/services/rest/';
  private readonly baseUrl = `${this.apiUrl}?api_key=${this.apiKey}&format=json&nojsoncallback=1&method=flickr.photos.`;
  private readonly flickrPhotoSearch = `${this.baseUrl}search&per_page=20&tags=`;

  constructor(private http: HttpClient, private token: TokenService) { }

  getSearchResults(page: number, term = null): Observable<{}> {
    return this.getFlickrPhotoSearch(term, page, '');
  }

  getFlickrPhotoSearch(term, page, tagMode) {
    return this.http.get(`${this.flickrPhotoSearch}${encodeURIComponent(term)}&page=${page}${tagMode}`);
  }

    // ToDo: rename...
  getPhotoThumnail(data): FlickrUrlsViewModel[] {
    const urls: FlickrUrlsViewModel[] = [];

    data.forEach((element, index) => {
      urls.push({
        id: index + 1,
        url: `https://farm${element.farm}.staticflickr.com/${element.server}/${element.id}_${element.secret}_n.jpg`
      });
    });

    return urls;
  }
}
