-- Drop tables to clean the DB
DROP TABLE IF EXISTS Company;
DROP TABLE IF EXISTS CompanyPriceHistory;

--Table responsible to hold company tickers and basic data
CREATE TABLE Company (
	id				INT				NOT NULL    IDENTITY    PRIMARY KEY,
	symbol			VARCHAR(10)		NOT NULL,
	friendlyName	VARCHAR(100)	NOT NULL,
	type			VARCHAR(4)		NOT NULL,
	currency		VARCHAR(5)		NOT NULL,
	companyLogo		VARCHAR(200)	NULL,
	CONSTRAINT UK_Company_Symbol UNIQUE(symbol)   
);


-- Company prices history
CREATE TABLE CompanyPriceHistory (
	id								INT				NOT NULL	IDENTITY    PRIMARY KEY,
	companyId						INT				NOT NULL,
	date							date			NOT NULL,
	openPrice						float	NOT NULL,
	d1VariationPercentage			float	NOT NULL,
	firstPriceVariationPercentage	float

	CONSTRAINT FK_CompanyPriceHistory_Company FOREIGN KEY (companyId)
		REFERENCES Company (id)    
		ON DELETE CASCADE 
);
