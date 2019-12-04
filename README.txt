Happy holidays! I created this little application to make pairing in bulk a lot faster for our subreddit's Secret Santa event.

To install: See Releases page.

1. Import the file of users.
--- Must be tab-delimited (TSV).

2. Import the file of Grinches / banned users.
--- Must be tab-delimited (TSV).

3. Click the Parse button. Go make a sandwich, this takes a while for a lot of accounts.
--- The app will organize users into groups of Santas and reject any users who match as Grinches or whose accounts are fewer than 90 days old.
--- If a user has an unparsable account (i.e. invalid username or an "18+ only" user page) they'll be sorted for manual review.

4. If any users require manual review, the Manual Review tab will be enabled and populated. You'll have to look up their accounts on your own. Uncheck the box next to their name and click the button below to add them into the "valid Santas" pool.
--- Peruse the other tabs for Booted users and Valid Santas if you wish to see some logging.

5. Hit the Pair/Export button and save your export file.
--- The app will pair valid Santas within their own countries and write the pairs to the export file.
--- If a country has an odd number of Santas, it will try to grab a random Santa who is okay with shipping overseas.
---- Failing that, it will try to grab a random Santa who is at least okay with shipping internationally.
----- As a last resort, a random Santa will be grabbed for the "international pool" for pairing.

6. If any Santas require manual pairing, the Manual Pairing tab will be enabled and populated. Select a Giver and Recipient, then hit the Match button.
--- Matching a Santa as their own Giver and Recipient will not work.
--- If only one Santa requires a manual pairing, a special entry is shown instead. :)
--- Be thoughtful. If a Santa can't ship overseas as a Giver, try not to pair them with an overseas Recipient.
--- You will have to start again if you reduce the list to a single Santa pairing to themselves. Sorry - run the parse again (Step 5).

7. When pairing is complete, you're done! Go inspect the export file.

===============
Expected column order for import file of users:
-----------------------------------------------
Timestamp
First Name
Last Name
Email Address
Reddit username
Wishlist
Would you like to be a rematcher? (Yes/No)
Are you willing to ship internationally? (Yes/No)
Are you willing to ship overseas? (Yes/No)
Country
Address

Expected column order for import file of Grinches (banned users):
-----------------------------------------------------------------
Full Name
Email Address
Reddit username
Country
Address

Export column order (headers are not included):
-----------------------------------------------
[Giver] First Name
[Giver] Last Name
[Giver] Email Address
[Giver] Reddit username
[Giver] Wishlist
[Giver] Country
[Giver] Address
[Recipient] First Name
[Recipient] Last Name
[Recipient] Email Address
[Recipient] Reddit username
[Recipient] Wishlist
[Recipient] Country
[Recipient] Address
