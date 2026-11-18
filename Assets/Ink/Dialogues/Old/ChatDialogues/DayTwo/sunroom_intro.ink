VAR nextChatArea = ""
VAR nextBranch = ""

# speaker: SolBot
# portrait: solbot_portrait
\-- You’ve been added by Ra to \#sun-room \--

# speaker: HulioVerr
# portrait: hulioverr_portrait
hey hey!
you made it here, nice (~‾⌣‾)~

# speaker: C0deRuse
# portrait: coderuse_portrait
yoo welcome to the secret corner of the server

# speaker: HulioVerr
# portrait: hulioverr_portrait
its not that secret, dude
it's just our little chill spot

# speaker: C0deRuse
# portrait: coderuse_portrait
yea i know
sounds cooler when i say "secret"

# speaker: Rael
# portrait: rael_portrait
Glad that you find your way here, Player_Name
Thought you might like a bit of quiet first

# speaker: C0deRuse
# portrait: coderuse_portrait
seriously
last time someone was spamming gifs for like ten minutes straight

# speaker: HulioVerr
# portrait: hulioverr_portrait
that was me, actually

# speaker: C0deRuse
# portrait: coderuse_portrait
...of course, it was

# speaker: Rael
# portrait: rael_portrait
You two just never stop, huh?

# speaker: HulioVerr
# portrait: hulioverr_portrait
we're just making sure our new friend feels the energy around here

# speaker: Rael
# portrait: rael_portrait
The good kind of energy, i hope

# speaker: Player
# portrait: player_portrait
+[you all seem really close] -> branch1
+[i can feel the good energy here] ->branch2

==branch1==
# speaker: HulioVerr
# portrait: hulioverr_portrait
yeah, we've been hanging out here for a while
kind of become a family, y'know?

# speaker: C0deRuse
# portrait: coderuse_portrait
"family" is code "people who put up with my"

# speaker: Rael
# portrait: rael_portrait
This whole thing kinda started with the four of us
The other one is my brother, but he's not available at the moment
You'll meet him when the time comes

->continue

==branch2==
# speaker: Rael
# portrait: rael_portrait
I'm glad to hear that
I hope you can find your solace here with us

# speaker: HulioVerr
# portrait: hulioverr_portrait
take it like this place is a little beach break between storms

# speaker: C0deRuse
# portrait: coderuse_portrait
...or like a nap you didn't know you needed

->continue

==continue==
# speaker: HulioVerr
# portrait: hulioverr_portrait
also, you can talk here first before jumping back in

# speaker: C0deRuse
# portrait: coderuse_portrait
stay here as long as you want

# speaker: Rael
# portrait: rael_portrait
We like to welcome people slowly
I'll give you access to the other channels slowly
Some people may find our server a bit...overwhelming

# speaker: HulioVerr
# portrait: hulioverr_portrait
just wanna make a safe space for everyone

# speaker: Player
# portrait: player_portrait
+[you sound like you really care] -> branch3
+[that's nice of you guys] ->branch4
+[do you do this for everyone?] ->branch5

==branch3==
# speaker: Rael
# portrait: rael_portrait
I do
Everyone deserves a safe space, even if the world make it seem like they didn't

# speaker: HulioVerr
# portrait: hulioverr_portrait
hehe
he's such a softie

# speaker: C0deRuse
# portrait: coderuse_portrait
that's why everyone sticks around

->ending

==branch4==
# speaker: HulioVerr
# portrait: hulioverr_portrait
aww thank youu
we just try to pass the good stuff foward

# speaker: C0deRuse
# portrait: coderuse_portrait
he meant the good vibes, not the viruses on him

# speaker: HulioVerr
# portrait: hulioverr_portrait
HEY

->ending

==branch5==
# speaker: C0deRuse
# portrait: coderuse_portrait
well, some poeple just ghost after joining
so no ig?

# speaker: HulioVerr
# portrait: hulioverr_portrait
but if someone actually wants to talks like you
we make sure they're not alone in it

# speaker: Rael
# portrait: rael_portrait
Connection's kind of what keeps the light alive

->ending

==ending==
# speaker: C0deRuse
# portrait: coderuse_portrait
I hope you give us a chance

# speaker: HulioVerr
# portrait: hulioverr_portrait
Welcome to Solace, Player_Name

# speaker: Rael
# portrait: rael_portrait
~ nextChatArea = "ChatAreaLounge"
~ nextBranch = "to_be_continued"
We just want you to find your solace with us

-> END