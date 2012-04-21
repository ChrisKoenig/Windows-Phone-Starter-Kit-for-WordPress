<?php

/*
Plugin Name: Windows Phone Plugin for WordPress
Plugin URI: http://chriskoenig.net
Description: WordPress plugin from Microsoft that generates WIndows Phone 7 friendly RSS Feeds.
Version: 1.4
Date: 21 Apr 2012
Author: Chris Koenig(Microsoft), Kirk Ballou(Touch Titans)
Author URI: http://chriskoenig.net
*/ 

// update: 21 APR 2012 - cleaned up code and fixed an issue with the comment feed display and changed plugin name
// update: 10 JAN 2012 - updated to match the changes Caleb made to the original source

//Add the feeds 
add_action('init', 'myPlugin_add_feed');

function myPlugin_add_feed() {
  add_feed('categories', 'category_feed'); 
  add_feed('user_info', 'user_info_feed');
  add_feed('add_comment', 'add_comment_feed');
  add_feed('get_recent', 'load_recent_feed');
  add_feed('get_cat_feed', 'load_cat_feed');
  add_feed('get_comments_feed', 'load_guestb_by_id');
}

 
//comments feed 
function load_guestb_by_id() 
{ 
  $args = array(
      'numberposts'     => 10,
      'offset'          => 0,
      'category'        => '',
      'orderby'         => 'post_date',
      'order'           => 'DESC',
      'include'         => '',
      'exclude'         => '',
      'meta_key'        => '',
      'meta_value'      => '',
      'post_type'       => 'post',
      'post_mime_type'  => '',
      'post_parent'     => '',
      'post_status'     => 'publish' );

  $comment_array = get_approved_comments($_GET["post_id"]); 
  echo '<?xml version="1.0" encoding="UTF-8"?>'; 
?><rss version="2.0" 
       xmlns:content="http://purl.org/rss/1.0/modules/content/"
       xmlns:dc="http://purl.org/dc/elements/1.1/"
       xmlns:atom="http://www.w3.org/2005/Atom"
       xmlns:sy="http://purl.org/rss/1.0/modules/syndication/">
  <channel>
    <title><?php bloginfo_rss('name'); wp_title_rss(); ?></title>
    <link><?php bloginfo_rss('url') ?></link>
    <atom:link href="<?php self_link() ?>" rel="self" type="application/rss+xml" />
    <description><?php bloginfo_rss("description") ?></description>
<?php foreach( $comment_array as $post ){ ?>
    <item>
      <title><?php echo formatDateAndTime($post->comment_date); ?></title>
      <author><?php echo htmlspecialchars($post->comment_author); ?></author>
      <email><?php echo $post->comment_author_email; ?></email>
      <comment><?php  echo htmlspecialchars($post->comment_content); ?></comment>
    </item>
<?php } ?>
  </channel>
</rss><?php 
                } 
//get recent post feed
function load_recent_feed()
{ 
  $args = array(
      'numberposts'     => 10,
      'offset'          => 0,
      'category'        => '',
      'orderby'         => 'post_date',
      'order'           => 'DESC',
      'include'         => '',
      'exclude'         => '',
      'meta_key'        => '',
      'meta_value'      => '',
      'post_type'       => 'post',
      'post_mime_type'  => '',
      'post_parent'     => '',
      'post_status'     => 'publish' );

  $recent_posts = get_posts( $args );

  echo '<?xml version="1.0" encoding="UTF-8"?>'; 
?><rss version="2.0"
    xmlns:content="http://purl.org/rss/1.0/modules/content/"
    xmlns:dc="http://purl.org/dc/elements/1.1/"
    xmlns:atom="http://www.w3.org/2005/Atom"
    xmlns:sy="http://purl.org/rss/1.0/modules/syndication/">
    <channel>
      <title><?php bloginfo_rss('name'); wp_title_rss(); ?></title>
      <link><?php bloginfo_rss('url') ?></link>
      <atom:link href="<?php self_link() ?>" rel="self" type="application/rss+xml" />
      <description><?php bloginfo_rss("description") ?></description>
<?php foreach( $recent_posts as $post ){ ?>
      <item>
        <title><?php $title=htmlspecialchars($post->post_title); echo $title; ?></title>
        <author><?php  $user_info = get_userdata($post->post_author);echo $user_info->display_name;  ?></author>
        <description><![CDATA[<?php echo $post->post_content ?>]]></description>
        <tags><?php $category = get_the_category($post->ID); echo $category[0]->cat_name; ?></tags>
        <id><?php echo $post->ID ?></id> 
        <pubDate><?php echo formatDateAndTime($post->post_modified);?></pubDate>
      </item>
<?php } ?> 
  </channel> 
</rss>
<?php } 

//load feed by category id
function load_cat_feed()
{ 
  $args = array(
      'numberposts'     => 10,
      'offset'          => 0,
      'category'        => $_GET["cat_id"],
      'orderby'         => 'post_date',
      'order'           => 'DESC',
      'include'         => '',
      'exclude'         => '',
      'meta_key'        => '',
      'meta_value'      => '',
      'post_type'       => 'post',
      'post_mime_type'  => '',
      'post_parent'     => '',
      'post_status'     => 'publish' );

  $recent_posts = get_posts( $args );
  echo '<?xml version="1.0" encoding="UTF-8"?>'; 
?><rss version="2.0"
    xmlns:content="http://purl.org/rss/1.0/modules/content/"
    xmlns:dc="http://purl.org/dc/elements/1.1/"
    xmlns:atom="http://www.w3.org/2005/Atom"
    xmlns:sy="http://purl.org/rss/1.0/modules/syndication/">
    <channel>
      <title><?php bloginfo_rss('name'); wp_title_rss(); ?></title>
      <link><?php bloginfo_rss('url') ?></link>
      <atom:link href="<?php self_link() ?>" rel="self" type="application/rss+xml" />
      <description><?php $description = htmlentities(bloginfo_rss("description")); echo $description; ?></description>
<?php foreach( $recent_posts as $post ){ ?>
      <item>
        <title><?php $title=htmlspecialchars($post->post_title); echo $title; ?></title>
        <author><?php  $user_info = get_userdata($post->post_author);echo $user_info->display_name;  ?></author>
        <description><![CDATA[<?php echo $post->post_content ?>]]></description>
        <tags><?php $category = get_the_category($post->ID); echo $category[0]->cat_name; ?></tags>
        <id><?php echo $post->ID ?></id> 
        <pubDate><?php echo formatDateAndTime($post->post_modified); ?></pubDate>
      </item>
<?php } ?>
    </channel> 
</rss>

<?php }

//get categories feed
function category_feed()
{
    $args=array(
        'hide_empty' => 0,
        'orderby' => 'name',
        'order' => 'ASC'
    );

    $categories = get_categories($args);
    
    echo '<?xml version="1.0" encoding="UTF-8"?>'; 
?><rss version="2.0"
   xmlns:content="http://purl.org/rss/1.0/modules/content/"
   xmlns:dc="http://purl.org/dc/elements/1.1/"
   xmlns:atom="http://www.w3.org/2005/Atom"
   xmlns:sy="http://purl.org/rss/1.0/modules/syndication/">
   <channel>
    <title><?php bloginfo_rss('name'); wp_title_rss(); ?></title>
    <link><?php bloginfo_rss('url') ?></link>
    <atom:link href="<?php self_link() ?>" rel="self" type="application/rss+xml" />
    <description><?php bloginfo_rss("description") ?></description>
<?php foreach ($categories as $category) { ?>
    <item>
      <title><?php echo $category->name; ?></title>
      <id><?php echo $category->cat_ID; ?></id>
    </item>
<?php } ?>
  </channel> 
</rss><?php }; 


//get user info
function user_info_feed()
{
  // header('Content-Type: text/xml; charset='.get_option('blog_charset'), true);
  global $wpdb; 
  $user_id = 1;
  if (isset($_GET["user_id"]))
  {
    $user_id = $_GET["user_id"];
  }
  $current_user = get_userdata($user_id);    
  $gravatar = "http://www.gravatar.com/avatar/" . md5(strtolower($current_user->user_email));
  $bio = $wpdb->get_var($wpdb->prepare("SELECT meta_value FROM wp_usermeta WHERE meta_key = 'description' AND wp_usermeta.user_id = " . $current_user->ID . ";"));
  //          echo "$current_user is"; 
  echo '<?xml version="1.0" encoding="UTF-8"?>';?>
<UserInfo>
  <UserID><?php echo $current_user->ID; ?></UserID>
  <UserName><?php echo $current_user->user_login; ?></UserName>
  <FirstName><?php echo $current_user->user_firstname; ?></FirstName>
  <LastName><?php echo $current_user->user_lastname; ?></LastName>
  <DisplayName><?php echo $current_user->nickname; ?></DisplayName>
  <EmailAddress><?php echo $current_user->user_email; ?></EmailAddress>
  <Gravatar><?php echo $gravatar; ?></Gravatar>
  <Bio><?php echo $bio; ?></Bio>
</UserInfo>
<?php
}

//add a comment
function add_comment_feed()
{ 
  $comment_post_ID = $_GET["post_id"];
  $comment_author = $_GET["name"];
  $comment_author_email = $_GET["email"];
  $comment_author_url = ""; 
  $comment_content = $_GET["comment"]; 
  $comment_author_IP = ""; 
  $comment_agent = ""; 
  $time = date('Y-m-d'); 
  $data = array(
    'comment_post_ID' => $comment_post_ID,
    'comment_author' => $comment_author,
    'comment_author_email' => $comment_author_email,
    'comment_author_url' => $comment_author_url,
    'comment_content' => $comment_content,
    'comment_type' => '',
    'comment_parent' => 0,
    'user_id' => 0,
    'comment_author_IP' => $comment_author_IP,
    'comment_agent' => $comment_agent,
    'comment_date' => $time,
    'comment_approved' => '0'
  );

  wp_new_comment( $data ); 
   
}

function formatDateAndTime($dtm)
{
  $dateTime = new DateTime($dtm); 
  return date_format($dateTime, 'M jS Y');
}

?>