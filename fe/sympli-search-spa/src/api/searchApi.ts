// src/api/searchApi.ts
import axios from 'axios';
import { SearchRequest, SearchResult } from '../models';

const API_URL = 'https://localhost:7250/api/search';

export const searchKeyword = async (payload: SearchRequest): Promise<SearchResult> => {
  const url = `${API_URL}/${encodeURIComponent(payload.browser)}/${encodeURIComponent(payload.keyword)}/${encodeURIComponent(payload.url)}`;

  const response = await axios.get<SearchResult>(url, {
    headers: {
      'Content-Type': 'application/json',
    },
  });
  return response.data;
};

