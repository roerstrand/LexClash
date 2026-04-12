
Feature: Seeded Data 

Scenario: Seed categories and words
Given an empty database
When I seed categories 
And I seed countries
Then the database should contain 3 categories
And the database should contain at least 100 words