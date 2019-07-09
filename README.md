Why is this a thing? 
             
  During some user testing of one of our web applications they wanted us to display the ASP.Net Session ID's 
in the page header because reasons and some users were reporting back that we were "inserting foul language" 
and being very inappropriate.  

  If you're not familiar with how ASP.Net Session ID's are generated, they are just strings of random characters and after a lot of testing of the the string generation you could actually see some pretty offensive words. So we ended up having to write a custom session id manager that would parse for 'bad words' and regenerate the session id if it contained any matches.  

So when I was discussing this sillyness with a colleague of mine she joked that I should insert things on purpose.


             
