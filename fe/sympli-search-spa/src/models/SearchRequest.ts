import { Browser } from "../types";

export interface SearchRequest {
    keyword: string;
    url: string;
    browser: Browser;
  }