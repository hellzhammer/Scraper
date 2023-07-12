using System.Net;
using System.IO;
using System;

namespace WebScraper;
public class Scraper
{
    public const string base_url = "https://finance.yahoo.com/quote/";
    public const string mid_url = "?p=";

    public async Task<string> get_content(string symbol)
    {
        string url = base_url + symbol + mid_url + symbol;
        HttpClient cl = new HttpClient();
        var t_out = await cl.GetAsync(url);
        string cont = await t_out.Content.ReadAsStringAsync();
        cl.Dispose();
        return cont;
    }

    // collects all of the stocks data from yahoos page
    public async Task<Scraper_Model> Scrape(string stock_sym)
    {
        Scraper_Model model = new Scraper_Model();
        model.raw_data = await this.get_content(stock_sym);
        model.stock_symbol = stock_sym;
        //FIN_TICKER_PRICE&quot;:&quot;
        model.curr_value = 0;
        if(double.TryParse(this.find_value_search(model.raw_data, "FIN_TICKER_PRICE&quot;:&quot;", "&"), out double result)){
            model.curr_value = result;
        }
        //EPS_RATIO-value
        model.curr_eps = 0;
        if(double.TryParse(this.find_value_search(model.raw_data, "EPS_RATIO-value", "<").Split('>')[1], out double epsres)){
            model.curr_eps = epsres;
        }  
        //"PE_RATIO-value"
        model.curr_PE = 0;
        if(double.TryParse(this.find_value_search(model.raw_data, "PE_RATIO-value", "<").Split('>')[1], out double peres)){
            model.curr_PE = peres;
        }
        //ONE_YEAR_TARGET_PRICE-value
        model.one_year_target = 0;
        if(double.TryParse(this.find_value_search(model.raw_data, "ONE_YEAR_TARGET_PRICE-value", "<").Split('>')[1], out double tarres)){
            model.one_year_target = tarres;
        }
        //BETA_5Y-value
        model.beta = 0;
        if (double.TryParse(this.find_value_search(model.raw_data, "BETA_5Y-value", "<").Split('>')[1], out double bres))
        {
            model.beta = bres;
        }
        //MARKET_CAP-value
        model.mar_cap = this.find_value_search(model.raw_data, "MARKET_CAP-value", "<").Split('>')[1];
        //AVERAGE_VOLUME_3MONTH-value
        model.avg_volume = 0;
        if (double.TryParse(this.find_value_search(model.raw_data, "AVERAGE_VOLUME_3MONTH-value", "<").Split('>')[1], out double avgres))
        {
            model.avg_volume = avgres;
        }
        //FIFTY_TWO_WK_RANGE-value
        model.fifty_two_week_range = this.find_value_search(model.raw_data, "FIFTY_TWO_WK_RANGE-value", "<").Split('>')[1];
        //DAYS_RANGE-value
        model.days_range = this.find_value_search(model.raw_data, "DAYS_RANGE-value", "<").Split('>')[1];
        //OPEN-value
        model.open = 0;
        if(double.TryParse(this.find_value_search(model.raw_data, "OPEN-value", "<").Split('>')[1], out double openres)){
            model.open = openres;
        }
        //PREV_CLOSE-value
        model.prev_close = 0;
        if(double.TryParse(this.find_value_search(model.raw_data, "PREV_CLOSE-value", "<").Split('>')[1], out double closeres)){
            model.prev_close = closeres;
        }
        //D(ib) Fz(18px)
        model.symbol_title = this.find_value_search(model.raw_data, "D(ib) Fz(18px)", "<").Split('>')[1];
        
        return model;
    }

    public string find_value_search(string data, string split_val, string end_char)
    {
        var split = data.Split(split_val);
        string final = string.Empty;
        for (int i = 0; i < split[1].Length; i++)
        {
            if (split[1][i] == char.Parse(end_char))
            {
                break;
            }
            else
            {
                final += split[1][i];
            }
        }

        return final;
    }

    public string find_value_search(string data, string split_val, int chars_to_add)
    {
        var split = data.Split(split_val);
        string final = string.Empty;
        for (int i = 0; i < chars_to_add; i++)
        {
            final += split[1][i];
        }

        return final;
    }
}

public class Scraper_Model
{
    public string stock_symbol { get; set; }
    public string symbol_title { get; set; }
    public string raw_data { get; set; }
    public double curr_eps { get; set; }
    public double curr_PE { get; set; }
    public double one_year_target { get; set; }
    public double beta { get; set; }
    public string mar_cap { get; set; }
    public double avg_volume { get; set; }
    public string fifty_two_week_range { get; set; }
    public string days_range { get; set; }
    public double open { get; set; }
    public double prev_close { get; set; }
    public double curr_value { get; set; }
}